using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Application.Common;
using Application.Common.Interfaces;
using Application.Features.Admin.Models;
using Application.Features.Admin.Session.Queries;
using Application.Features.App.Answer.Models;
using Application.Features.App.File.Commands;
using Core;
using IronPdf;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

public interface IReportService
{
    Task<ReportViewModel> GenerateSessionReport(Guid sessionId);
    Tuple<AssessmentDataDTO, Dictionary<Guid, UserAnswerDetailViewModel>> DoAssessmentCalculationsAndPrepareQuestionAnswers(SessionDTO sessionDTO);
    Tuple<AssessmentDataDTO, Dictionary<Guid, UserAnswerDetailViewModel>> DoAssessmentCalculationsAndPrepareQuestionAnswers(Dictionary<Guid, AnswerDTO> questionMap);
}

public class ReportService : IReportService
{
    private readonly IMediator _mediator;
    private readonly IPdfService<ChromePdfRenderer> _pdfService;
    private readonly IBlobStorage _blobStorageService;
    private readonly ILogger<ReportService> _logger;
    private readonly ITemplateService _templateService;
    private const string MissingQuestionMessage = "Question missing";
    private const string MissingAnswerMessage = "Answer missing";

    private List<string> _pces = new List<string>();
    private List<string> _aces = new List<string>();

    private readonly Dictionary<string, string> EnvironmentalRiskFactorsFlags = new()
    {
        { ParentCharacteristics.SleepsOften, "Has a trusted adult who sleeps a lot" },
        { ParentCharacteristics.SickOften, "Lives with a trusted adult who is often sick" },
        { ParentCharacteristics.Overwhelmed, "Lives with a trusted adult who is overwhelmed" },
        // 
        // { ParentCharacteristics.HealthCondition, "Lives with someone who is seriously ill" },
        { ParentCharacteristics.CriesOften, "Lives with someone who cries often" },
        { ParentCharacteristics.Yells, "Lives with someone who yells" },
        { ParentCharacteristics.Smokes,"Lives with a smoker" }
    };

    private Dictionary<Guid, string> SocialRiskFactors = new()
    {
        {
            MappedQuestionIds.RaceSkinColorReligionQuestionId,
            "Worries/fears they are being treated differently because of race, skin color, religion or ethnicity"
        },
        {
            MappedQuestionIds.HaveYourPeersHurtYouQuestionId,
            "Has peers that have said/done things that hurt or made them feel bad."
        }
    };

    public class AdverseChildhoodExperiencesDescriptions
    {
        public const string SplitsTimeBetweenParents = "Splits their time living between parents";
        public const string LivesWithSeriouslyIllFamilyMember = "Lives with a mentally or physically ill family member";
        public const string CantTalkWithTrustedAdultAtTheirHome = "Can’t talk to a trusted adult in their home";
        public const string TreatedWithPhysicalAnger = "Has been treated with physical anger";
        public const string CriticizedOrSwornAt = "Is criticized and/or sworn at";
        public const string LivesWithDrinkerOrDrugUsers = "Lives with someone who drinks too much or uses illegal drugs";
        public const string ParentHasTroubleWithLaw = "Has a parent who has served time in jail or is in trouble with the law";
        public const string LivesWithSomeoneWhoActsOutInAnger = "Lives with someone who acts out in anger";
    }


    public class PositiveChildhoodExperienceDescriptions
    {
        public const string TrustedAdultWhoListens = "Feels they have at least 1 trusted adult who listens to them";
        public const string TrustedAdultWhoSpendsTime = "Feels they have at least 1 trusted adult who spends time with them";
        public const string AdultWhoMaintainsFamilyRelationships = "Sees they have adults who maintain family relationships";
        public const string FamilyMembersCountedOn = "Family members they could count on";
        public const string PeopleAboveAge18OutsideFamily = "Has people age 18+ outside the family they can count on";
        public const string FriendsAreSupportive = "Doesn't have friends who hurt them or make them feel bad";
        public const string WhatGivesEnergy = "What gives you energy?";
    };

    public class RiskAssessedDescriptions
    {
        public const string Adhd = "ADHD";
        public const string Suicidality = "Suicidality";
    }

    public static readonly List<string> ParentsFeProtectiveIfNotFoundCharacteristics = new()
    {
        ParentCharacteristics.Angry,
        ParentCharacteristics.Anxious,
        ParentCharacteristics.Irritable,
        ParentCharacteristics.Moody,
        ParentCharacteristics.Overwhelmed,
        ParentCharacteristics.Sad,
        ParentCharacteristics.Tired,

        ParentCharacteristics.HardToConverseWith,
        ParentCharacteristics.Workaholic,
        ParentCharacteristics.Distracted,
        ParentCharacteristics.Judgmental,
        ParentCharacteristics.CriesOften,
        ParentCharacteristics.Critical,
        ParentCharacteristics.Sarcastic,
        ParentCharacteristics.Yells,
        ParentCharacteristics.AlcoholicOrDrugUser,
        ParentCharacteristics.Smokes,
        ParentCharacteristics.Swears,
        ParentCharacteristics.Overweight,
        ParentCharacteristics.SleepsOften,
        ParentCharacteristics.LawOffender,
        ParentCharacteristics.SickOften,
        ParentCharacteristics.OftenInTrouble
    };

    public static readonly List<string> SiblingsFeProtectiveNotFoundCharacteristics = new()
    {
        SiblingCharacteristics.Irritable,
        SiblingCharacteristics.Moody,
        SiblingCharacteristics.InTrouble
    };

    public ReportService(IMediator mediator, IPdfService<ChromePdfRenderer> pdfService,
        IBlobStorage blobStorageService, ILogger<ReportService> logger, ITemplateService templateService)
    {
        _mediator = mediator;
        _pdfService = pdfService;
        _blobStorageService = blobStorageService;
        _logger = logger;
        _templateService = templateService;
    }

    /// <summary>
    /// Generate TWS Student Report
    /// </summary>
    /// <remarks>
    /// Only one report is created for a session. However, it is possible to rerun report generation
    /// and update the report contents with associated DB record
    /// </remarks>
    /// <param name="sessionId">Session ID</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException">Must be a SessionId and not an Empty GUID</exception>
    public async Task<ReportViewModel> GenerateSessionReport(Guid sessionId)
    {
        _logger.LogInformation("Starting report generation for session {SessionId}", sessionId);

        if (sessionId == Guid.Empty)
            throw new ArgumentException($"Empty GUID ({Guid.Empty}) is not a valid sessionId", nameof(sessionId));

        var sessionDTO = await _mediator
            .Send(new GetLastSessionWithQuestionsAndAnswers { SessionId = sessionId });

        // Do report rule evaluations
        var viewModel = PrepareViewModel(sessionDTO);

        var html = await _templateService.CompileRenderAsync(ReportTemplate.MainTemplate, viewModel);
        // var html = await razorLite.CompileRenderAsync("ReportTemplate", viewModel);

        // Create Report PDF
        var date = DateTime.UtcNow.ToString("yyyy-MM-dd");
        var pdfFileName = $"Tws.Report.{sessionDTO.Id}.{date}.pdf";

        // Save PDF file into temporary directory. The file is ephemeral and will be uploaded to the cloud.
        // Once uploaded we'll remove the file.
        var pdfFilePath = TempFile.GetFullyQualifiedPath(pdfFileName);

        _pdfService.Renderer.RenderingOptions.CustomCssUrl = _templateService
            .GetFullyQualifiedPath(ReportTemplate.Styles.ReportTemplate, includeFile: true);

        // _pdfService.Renderer.RenderingOptions.HtmlFooter = new HtmlHeaderFooter()
        // {
        //     LoadStylesAndCSSFromMainHtmlDocument = true,
        //     HtmlFragment = await _templateService.CompileRenderPartialAsync(ReportTemplate.Partial.Footer, new {})
        // };
        _pdfService.SaveDocument(pdfFilePath, html!);

        // Upload file to cloud storage
        _blobStorageService.SetStorageAccount(BlobStorageType.Client);

        using var stream = new StreamReader(pdfFilePath);

        var normalizedLocationId = sessionDTO.LocationId.ToString().ToLowerInvariant();
        var absolutePath = $"{StorageContainers.ReportsStorage.Name}/{normalizedLocationId}";

        _logger.LogInformation("Uploading generated report {ReportPath} to storage", $"{absolutePath}/{pdfFileName}");

        await _blobStorageService
            .UploadBlobAsync(stream.BaseStream, absolutePath, pdfFileName,
                overwrite: true);

        // Save file to DB.
        var fileInfo = new FileInfo(pdfFilePath);
        var fileDTO = await _mediator.Send(new GetFileBySessionId { SessionId = sessionId });
        var checksum = stream.BaseStream.Checksum(ChecksumAlgorithms.Md5);

        // If the file exists and has the same date, we'll update the checksum to reflect document
        // changes. However, if the document is re-generated on another day, we create a new 
        // record to represent a new document in blob storage. This function exists to 
        // assisting with auditing system functions.
        if (fileDTO is null || !IsSameDay(fileDTO.CreatedAt, DateTime.UtcNow))
        {
            _logger.LogInformation("Saving session ({SessionId}) report reference to DB", sessionId);

            await _mediator.Send(new AddFile
            {
                SessionId = sessionId,
                FileDTO = new FileDTO
                {
                    Category = "Report",
                    MimeType = System.Net.Mime.MediaTypeNames.Application.Pdf,
                    BlobName = pdfFileName,
                    FileName = pdfFileName,
                    FileSize = fileInfo.Length,
                    FileHash = checksum,
                    DisplayName = $"TWS Report {date}",
                    Description = $"TWS report for student {viewModel.Name} generated on {date} for session {sessionId}",
                    StorageAccount = _blobStorageService.StorageAccountName,
                    StorageContainer = StorageContainers.ReportsStorage.Name
                }
            });
        }
        else if (fileDTO.FileHash != checksum)
        {
            _logger.LogInformation("Updating storage reference in DB");

            fileDTO.FileSize = fileInfo.Length;
            fileDTO.FileHash = checksum;
            fileDTO.DisplayName = $"TWS Report {date}";
            fileDTO.Description =
                $"TWS report for student {viewModel.Name} generated on {date} for session {sessionId}";

            await _mediator.Send(new EditFile { FileDTO = fileDTO });
        }

        // At this point the PDF has been uploaded to cloud storage and reference saved in the DB
        // We have no more use for this file, so we'll remove it.
        TempFile.DeleteFile(pdfFilePath);

        _logger.LogInformation("Removed temporary file {FilePath} for {SessionId}", pdfFilePath, sessionId);

        return viewModel;
    }

    private bool IsSameDay(DateTime dateA, DateTime dateB)
    {
        return dateA.Year == dateB.Year && dateA.Month == dateB.Month && dateA.Day == dateB.Day;
    }

    public AnswerPayloadDTO<T>? GetAnswerIfExistsOrNull<T>(Dictionary<Guid, AnswerDTO> answers, Guid questionId)
    {
        if (!answers.ContainsKey(questionId)) return null;

        var answer = answers[questionId];

        try
        {
            _logger.LogDebug("Deserializing answer {AnswerId} associated with question {QuestionId}",
                answer.Id, answer.QuestionId);

            return Utility.DeserializeObject<AnswerPayloadDTO<T>>(answer.Data);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Cannot deserialize answer {AnswerId} associated with question {QuestionId}",
                answer.Id, answer.QuestionId);

            return null;
        }

    }

    public void HandleAnswer<T>(Dictionary<Guid, AnswerDTO> answers, Guid questionId, Action<AnswerPayloadDTO<T>?> action)
    {
        var answerPayload = GetAnswerIfExistsOrNull<T>(answers, questionId);
        action(answerPayload);
    }

    public ReportViewModel PrepareViewModel(SessionDTO sessionDTO)
    {
        if (sessionDTO is null)
            throw new ArgumentNullException(nameof(sessionDTO));

        if (sessionDTO.Student is null)
            throw new NullReferenceException("Cannot generate report because required Student details are missing");

        var answeredQuestionMap = sessionDTO.Answers
            .ToDictionary(x => x.QuestionId);

        AnswerDTO? answerDTO;

        var reportViewModel = new ReportViewModel();
        var environmentalRiskFactorsFlags = new List<string>();
        var socialRiskFactorFlags = new Dictionary<Guid, FactorFlagsViewModel>();
        var hasAdhd = false;
        var peopleOverTheAgeOf18 = Enumerable.Empty<string>();

        _aces = new List<string>();
        _pces = new List<string>();

        HandleAnswer<IReadOnlyList<AnswerDataParentMood>>(answeredQuestionMap,
            MappedQuestionIds.ParentDescribeTheirMoodQuestionId,
            answerPayloadDTO =>
            {
                var characteristicsCollection = answerPayloadDTO
                    ?.Data?.Select(guardian => guardian.Characteristics)
                    .Where(characteristics => !answerPayloadDTO.Skipped && characteristics.Any())
                ?? Enumerable.Empty<List<string>>();

                if (answerPayloadDTO is { Skipped: false, Data: { } })
                    foreach (var characteristics in characteristicsCollection)
                    {
                        foreach (var characteristic in characteristics)
                        {
                            if (EnvironmentalRiskFactorsFlags.ContainsKey(characteristic))
                                environmentalRiskFactorsFlags
                                    .Add(EnvironmentalRiskFactorsFlags[characteristic]);
                        }

                        // if (characteristics.Contains(ParentCharacteristics.HealthCondition))
                        //     aces.Add(AdverseChildhoodExperiencesDescriptions.LivesWithSeriouslyIllFamilyMember);

                        // Changed to environmental flag area as requested by Wendy.
                        if (characteristics.Contains(ParentCharacteristics.HardToConverseWith))
                            environmentalRiskFactorsFlags.Add(AdverseChildhoodExperiencesDescriptions.CantTalkWithTrustedAdultAtTheirHome);


                        if (characteristics.Contains(ParentCharacteristics.OftenInTrouble))
                            _aces.Add(AdverseChildhoodExperiencesDescriptions.LivesWithSomeoneWhoActsOutInAnger);

                        // TODO - how do we know if a student has been treated with physical anger?
                        // if (characteristics.Contains(ParentCharacteristics.))

                        // Adverse Childhood Experiences
                        if (characteristics.Contains(ParentCharacteristics.Critical) ||
                            characteristics.Contains(ParentCharacteristics.Swears))
                            _aces.Add(AdverseChildhoodExperiencesDescriptions.CriticizedOrSwornAt);

                        if (characteristics.Contains(ParentCharacteristics.AlcoholicOrDrugUser))
                            _aces.Add(AdverseChildhoodExperiencesDescriptions.LivesWithDrinkerOrDrugUsers);

                        if (characteristics.Contains(ParentCharacteristics.LawOffender))
                            _aces.Add(AdverseChildhoodExperiencesDescriptions.ParentHasTroubleWithLaw);

                        // Positive Childhood Experiences
                        if (characteristics.Contains(ParentCharacteristics.Listens))
                            _pces.Add(PositiveChildhoodExperienceDescriptions.TrustedAdultWhoListens);

                        if (characteristics.Contains(ParentCharacteristics.SpendTimeTogether))
                            _pces.Add(PositiveChildhoodExperienceDescriptions.TrustedAdultWhoSpendsTime);

                        if (characteristics.Contains(ParentCharacteristics.PositiveFamilyRelationship))
                            _pces.Add(PositiveChildhoodExperienceDescriptions.AdultWhoMaintainsFamilyRelationships);
                    }
            });

        // Prepare environmental risk factors and ACEs for report
        // if (answeredQuestionMap.ContainsKey(MappedQuestionIds.ParentDescribeTheirMoodQuestionId))
        // {
        //     answerDTO = answeredQuestionMap[MappedQuestionIds.ParentDescribeTheirMoodQuestionId];
        //     var answerPayloadDTO =
        //         Utility.DeserializeObject<AnswerPayloadDTO<IReadOnlyList<AnswerDataParentMood>>>(answerDTO.Data);
        //     
        //     var characteristicsCollection = answerPayloadDTO
        //         ?.Data?.Select(guardian => guardian.Characteristics)
        //         .Where(characteristics => !answerPayloadDTO.Skipped && characteristics.Any())
        //         ?? Enumerable.Empty<List<string>>();
        //
        //     if (answerPayloadDTO is { Skipped: false, Data: { } })
        //         foreach (var characteristics in characteristicsCollection)
        //         {
        //             foreach (var characteristic in characteristics)
        //             {
        //                 if (EnvironmentalRiskFactorsFlags.ContainsKey(characteristic))
        //                     environmentalRiskFactorsFlags
        //                         .Add(EnvironmentalRiskFactorsFlags[characteristic]);
        //             }
        //             
        //             // if (characteristics.Contains(ParentCharacteristics.HealthCondition))
        //             //     aces.Add(AdverseChildhoodExperiencesDescriptions.LivesWithSeriouslyIllFamilyMember);
        //             
        //             // Changed to environmental flag area as requested by Wendy.
        //             if (characteristics.Contains(ParentCharacteristics.HardToConverseWith))
        //                environmentalRiskFactorsFlags.Add(AdverseChildhoodExperiencesDescriptions.CantTalkWithTrustedAdultAtTheirHome);
        //             
        //             
        //             if (characteristics.Contains(ParentCharacteristics.OftenInTrouble))
        //                 _aces.Add(AdverseChildhoodExperiencesDescriptions.LivesWithSomeoneWhoActsOutInAnger);
        //             
        //             // TODO - how do we know if a student has been treated with physical anger?
        //             // if (characteristics.Contains(ParentCharacteristics.))
        //             
        //             // Adverse Childhood Experiences
        //             if (characteristics.Contains(ParentCharacteristics.Critical) || 
        //                 characteristics.Contains(ParentCharacteristics.Swears))
        //                 _aces.Add(AdverseChildhoodExperiencesDescriptions.CriticizedOrSwornAt);
        //             
        //             if (characteristics.Contains(ParentCharacteristics.AlcoholicOrDrugUser))
        //                 _aces.Add(AdverseChildhoodExperiencesDescriptions.LivesWithDrinkerOrDrugUsers);
        //             
        //             if (characteristics.Contains(ParentCharacteristics.LawOffender))
        //                 _aces.Add(AdverseChildhoodExperiencesDescriptions.ParentHasTroubleWithLaw);
        //             
        //             // Positive Childhood Experiences
        //             if (characteristics.Contains(ParentCharacteristics.Listens))
        //                 _pces.Add(PositiveChildhoodExperienceDescriptions.TrustedAdultWhoListens);
        //
        //             if (characteristics.Contains(ParentCharacteristics.SpendTimeTogether))
        //                 _pces.Add(PositiveChildhoodExperienceDescriptions.TrustedAdultWhoSpendsTime);
        //             
        //             if (characteristics.Contains(ParentCharacteristics.PositiveFamilyRelationship))
        //                 _pces.Add(PositiveChildhoodExperienceDescriptions.AdultWhoMaintainsFamilyRelationships);
        //         }
        // }

        HandleAnswer<int>(answeredQuestionMap, MappedQuestionIds.RaceSkinColorReligionQuestionId, answerPayload =>
        {
            if (!socialRiskFactorFlags.ContainsKey(MappedQuestionIds.RaceSkinColorReligionQuestionId) &&
                answerPayload is { Skipped: false, Data: >= 4 })
            {
                socialRiskFactorFlags.Add(
                    MappedQuestionIds.RaceSkinColorReligionQuestionId,
                    new FactorFlagsViewModel
                    {
                        Label = SocialRiskFactors[MappedQuestionIds.RaceSkinColorReligionQuestionId],
                    });
            }
        });

        // var raceAnswerPayload =
        //     GetAnswerIfExistsOrNull<int>(answeredQuestionMap, MappedQuestionIds.RaceSkinColorReligionQuestionId);
        //
        // if (!socialRiskFactorFlags.ContainsKey(MappedQuestionIds.RaceSkinColorReligionQuestionId) && 
        //     raceAnswerPayload is { Skipped: false, Data: >= 4 })
        // {
        //     socialRiskFactorFlags.Add(
        //         MappedQuestionIds.RaceSkinColorReligionQuestionId, 
        //         new FactorFlagsViewModel
        //         {
        //             Label = SocialRiskFactors[MappedQuestionIds.RaceSkinColorReligionQuestionId],
        //         });
        // }

        // // Prepare social risk factors step #1
        // if (answeredQuestionMap.ContainsKey(MappedQuestionIds.RaceSkinColorReligionQuestionId))
        // {
        //     if (!socialRiskFactorFlags.ContainsKey(MappedQuestionIds.RaceSkinColorReligionQuestionId))
        //     {
        //         answerDTO = answeredQuestionMap[MappedQuestionIds.RaceSkinColorReligionQuestionId];
        //         var answerPayloadDTO = Utility.DeserializeObject<AnswerPayloadDTO<int>>(answerDTO.Data);
        //
        //         if (answerPayloadDTO is { Skipped: false, Data: >= 4 })
        //             socialRiskFactorFlags.Add(
        //                 MappedQuestionIds.RaceSkinColorReligionQuestionId, 
        //                 new FactorFlagsViewModel
        //                 {
        //                     Label = SocialRiskFactors[MappedQuestionIds.RaceSkinColorReligionQuestionId],
        //                 });
        //     }
        // }

        HandleAnswer<bool>(answeredQuestionMap, MappedQuestionIds.HaveYourPeersHurtYouQuestionId, answerPayloadDTO =>
        {
            if (answerPayloadDTO is null ||
                socialRiskFactorFlags.ContainsKey(MappedQuestionIds.HaveYourPeersHurtYouQuestionId)) return;

            // If the student provided the details get them for the report.
            var answerFreeformDTO = answeredQuestionMap.ContainsKey(MappedQuestionIds.WhatDidYouPeersDoToHurtYouQuestionId)
                ? answeredQuestionMap[MappedQuestionIds.WhatDidYouPeersDoToHurtYouQuestionId]
                : null;

            var answerPayloadFreeformDTO = Utility
                .DeserializeObject<AnswerPayloadDTO<string>>(answerFreeformDTO?.Data);

            var details = answerPayloadFreeformDTO.Data;

            if (answerPayloadDTO is { Skipped: true } or { Data: false })
                _pces.Add(PositiveChildhoodExperienceDescriptions.FriendsAreSupportive);

            if (answerPayloadDTO is { Skipped: false, Data: true })
                socialRiskFactorFlags.Add(
                    MappedQuestionIds.HaveYourPeersHurtYouQuestionId,
                    new FactorFlagsViewModel
                    {
                        Label = SocialRiskFactors[MappedQuestionIds.HaveYourPeersHurtYouQuestionId],
                        Details = details
                    });
        });

        HandleAnswer<AnswerDataLivingSituationDTO>(answeredQuestionMap, MappedQuestionIds.WhoDoYouLiveWithQuestionId, answerPayloadDTO =>
        {
            if (answerPayloadDTO is null or { Skipped: true }) return;

            var livingSituation = answerPayloadDTO.Data?.LivingSituation;
            var livesWithGrandParentsToo = answerPayloadDTO?.Data?.GrandParentsLiveAtResidence ?? false;
            var otherRelationship = answerPayloadDTO?.Data?.OtherRelationship;

            reportViewModel.LivesWith = livingSituation switch
            {
                LivingSituation.LivesWithBothParents => livesWithGrandParentsToo
                    ? "Lives with both parents, and has grand parents living with them"
                    : "Lives with both parents",
                LivingSituation.SplitTimeEvenly => livesWithGrandParentsToo
                    ? "Splits time evenly between parents, and has grand parents living with them"
                    : "Splits time evenly between parents",
                LivingSituation.SplitTimeMainlyLiveWith => livesWithGrandParentsToo
                    ? "Splits time with parents, but mainly lives with one, and has grand parents living with them"
                    : "Splits time with parents, but mainly lives with one",
                LivingSituation.LivesWithOneParent => livesWithGrandParentsToo
                    ? "Lives with one parent, and has grandparents living with them"
                    : "Lives with one parent",
                LivingSituation.LivesWithGrandparents => "Lives with grandparent(s)",
                LivingSituation.SiblingsAreCareTakers => "Siblings are care takers",
                LivingSituation.LivesWithSomeoneOtherThanFamily => string.IsNullOrWhiteSpace(otherRelationship)
                    ? "Lives with someone other than family"
                    : $"Lives with: {otherRelationship}",

                _ => "Unknown living situation"
            };

            var aceText = livingSituation switch
            {
                LivingSituation.SplitTimeEvenly => "Splits time equally between both parents",
                LivingSituation.SplitTimeMainlyLiveWith => "Splits time between both parents but mainly lives with one",
                LivingSituation.LivesWithOneParent => "Lives with one parent",
                _ => null
            };

            if (aceText is not null) _aces.Add(aceText);
        });


        // Prepare more ACEs for report
        // if (answeredQuestionMap.ContainsKey(MappedQuestionIds.WhoDoYouLiveWithQuestionId))
        // {
        //     answerDTO = answeredQuestionMap[MappedQuestionIds.WhoDoYouLiveWithQuestionId];
        //     var answerPayloadDTO = Utility
        //         .DeserializeObject<AnswerPayloadDTO<AnswerDataLivingSituationDTO>>(answerDTO.Data);
        //
        //     if (!answerPayloadDTO.Skipped)
        //     {
        //         var livingSituation = answerPayloadDTO.Data?.LivingSituation;
        //         var livesWithGrandParentsToo = answerPayloadDTO?.Data?.GrandParentsLiveAtResidence ?? false;
        //         var otherRelationship = answerPayloadDTO?.Data?.OtherRelationship;
        //         
        //         reportViewModel.LivesWith = livingSituation switch
        //         {
        //             LivingSituation.LivesWithBothParents => livesWithGrandParentsToo 
        //                 ? "Lives with both parents, and has grand parents living with them"
        //                 : "Lives with both parents",
        //             LivingSituation.SplitTimeEvenly => livesWithGrandParentsToo
        //                 ? "Splits time evenly between parents, and has grand parents living with them"
        //                 : "Splits time evenly between parents",
        //             LivingSituation.SplitTimeMainlyLiveWith => livesWithGrandParentsToo
        //                 ? "Splits time with parents, but mainly lives with one, and has grand parents living with them"
        //                 : "Splits time with parents, but mainly lives with one",
        //             LivingSituation.LivesWithOneParent => livesWithGrandParentsToo
        //                 ? "Lives with one parent, and has grandparents living with them"
        //                 : "Lives with one parent",
        //             LivingSituation.LivesWithGrandparents => "Lives with grandparent(s)",
        //             LivingSituation.SiblingsAreCareTakers => "Siblings are care takers",
        //             LivingSituation.LivesWithSomeoneOtherThanFamily => string.IsNullOrWhiteSpace(otherRelationship)
        //                 ? "Lives with someone other than family"
        //                 : $"Lives with: {otherRelationship}",
        //             
        //             _ => "Unknown living situation"
        //         };
        //
        //         var aceText = livingSituation switch
        //         {
        //             LivingSituation.SplitTimeEvenly => "Splits time equally between both parents",
        //             LivingSituation.SplitTimeMainlyLiveWith => "Splits time between both parents but mainly lives with one",
        //             LivingSituation.LivesWithOneParent => "Lives with one parent",
        //             _ => null
        //         };
        //         
        //         if (aceText is not null) _aces.Add(aceText);
        //     }
        // }

        // Prepare more PCEs for report
        HandleAnswer<IReadOnlyList<string>>(answeredQuestionMap, MappedQuestionIds.WhoCanYouCountOnInTheFamilyQuestionId,
            answerPayloadDTO =>
            {
                if (answerPayloadDTO is { Skipped: false })
                {
                    var totalFamilyMembers = (answerPayloadDTO.Data?.Count ?? 0) > 0 ? "Yes" : "No";
                    _pces.Add($"{PositiveChildhoodExperienceDescriptions.FamilyMembersCountedOn}: {totalFamilyMembers}");
                }
                else
                {
                    reportViewModel.FeFeRiskTotal += 1;
                }
            });

        HandleAnswer<IReadOnlyList<AnswerDataSiblingsDTO>>(answeredQuestionMap, MappedQuestionIds.WhatAboutSiblingsQuestionId,
            answerPayloadDTO =>
            {
                if (answerPayloadDTO is null or { Skipped: true }) return;

                reportViewModel.TotalSiblings = answerPayloadDTO.Data?.Count ?? 0;
            });

        HandleAnswer<IReadOnlyList<AnswerDataOutsideFamilyCanCountOnDTO>>(answeredQuestionMap,
            MappedQuestionIds.AreTherePeopleOutsideYouCanCountOnQuestionId,
            answerPayloadDTO =>
            {
                if (answerPayloadDTO is null or { Skipped: true }) return;

                peopleOverTheAgeOf18 = (answerPayloadDTO.Data?
                    .Where(e => e.OverAge18 == true)
                    .Select(x => x.Name) ?? Array.Empty<string?>()).ToArray()!;

                if (peopleOverTheAgeOf18.Any())
                    _pces.Add(PositiveChildhoodExperienceDescriptions.PeopleAboveAge18OutsideFamily);
            });

        HandleAnswer<IReadOnlyList<AnswerDataOutsideFamilyCanCountOnDTO>>(answeredQuestionMap,
            MappedQuestionIds.AreTherePeopleOutsideYouCanCountOnQuestionId,
            answerPayloadDTO =>
            {
                if (answerPayloadDTO is not null or { Skipped: true }) return;

                peopleOverTheAgeOf18 = (answerPayloadDTO?.Data?
                    .Where(e => e.OverAge18 == true)
                    .Select(x => x.Name) ?? Array.Empty<string?>()).ToArray()!;

                if (peopleOverTheAgeOf18.Any())
                    _pces.Add(PositiveChildhoodExperienceDescriptions.PeopleAboveAge18OutsideFamily);
            });

        HandleAnswer<IReadOnlyList<AnswerDataOutsideFamilyCanCountOnDTO>>(answeredQuestionMap,
            MappedQuestionIds.AreTherePeopleOutsideYouCanCountOnQuestionId,
            answerPayloadDTO =>
            {
                if (answerPayloadDTO is null or { Skipped: true }) return;

                peopleOverTheAgeOf18 = (answerPayloadDTO.Data?
                    .Where(e => e.OverAge18 == true)
                    .Select(x => x.Name) ?? Array.Empty<string?>()).ToArray()!;

                if (peopleOverTheAgeOf18.Any())
                    _pces.Add(PositiveChildhoodExperienceDescriptions.PeopleAboveAge18OutsideFamily);
            });

        HandleAnswer<string>(answeredQuestionMap, MappedQuestionIds.WhatStrengthsGiftsTalentsQuestionId,
            answerPayloadDTO =>
            {
                if (answerPayloadDTO is { Skipped: false })
                    reportViewModel.ReportedGiftsStrengthsTalents = answerPayloadDTO.Data;
            });

        HandleAnswer<string?>(answeredQuestionMap, MappedQuestionIds.WhatEnergizesYouQuestionId, answerPayloadDTO =>
        {
            if (answerPayloadDTO is { Skipped: false } && !string.IsNullOrWhiteSpace(answerPayloadDTO.Data))
                reportViewModel.ReportedWhatGivesEnergy = answerPayloadDTO.Data;
        });

        HandleAnswer<bool>(answeredQuestionMap, MappedQuestionIds.SeeingMentalHealthProfessionallyQuestionId,
            answerPayloadDTO =>
            {
                if (answerPayloadDTO is null or { Skipped: true }) return;
                reportViewModel.IsSeeingMentalHealthProfessional = answerPayloadDTO.Data;
            });


        // Other Risks Flagged
        // Handle ADHD calculations

        HandleAnswer<IReadOnlyList<string>>(answeredQuestionMap, MappedQuestionIds.CheckAllThatApplyQuestionId, answerPayloadDTO =>
        {
            if (answerPayloadDTO is null or { Skipped: true }) return;

            var selections = answerPayloadDTO.Data ?? Array.Empty<string>();

            if (selections.Contains(CheckAllThatApply.CantControlThoughts))
                hasAdhd = true;
        });

        HandleAnswer<bool>(answeredQuestionMap, MappedQuestionIds.HardToConcentrateQuestionId, answerPayloadDTO =>
        {
            if (answerPayloadDTO is { Skipped: false })
            {
                hasAdhd = answerPayloadDTO.Data;
            }
        });


        if (hasAdhd)
        {
            reportViewModel.OtherRisksFlagged.Add(RiskAssessedDescriptions.Adhd);
        }

        HandleAnswer<bool>(answeredQuestionMap, MappedQuestionIds.SuicidalQuestionId, answerPayloadDTO =>
        {
            if (answerPayloadDTO is { Skipped: false, Data: true })
            {
                reportViewModel.OtherRisksFlagged.Add(RiskAssessedDescriptions.Suicidality);
            }
        });

        // Handle Suicidality calculation

        HandleAnswer<AnswerDataExperiencedDeathDTO>(answeredQuestionMap,
            MappedQuestionIds.ExperiencedDeathInTheFamilyQuestionId,
            answerPayloadDTO =>
            {
                if (answerPayloadDTO is { Skipped: false, Data: { Selected: "Yes" } })
                {
                    reportViewModel.HasDeathInFamily = true;
                    reportViewModel.DeathInTheFamilyWho = answerPayloadDTO.Data?.Who;
                    reportViewModel.DeathInTheFamilyWhen = answerPayloadDTO.Data?.When;
                }
            });

        reportViewModel.Name = $"{sessionDTO.Student.FirstName} {sessionDTO.Student.LastName}";
        reportViewModel.Grade = sessionDTO.Student.GradeLevel.ToString();
        reportViewModel.AssessmentDate = sessionDTO.CreatedAt;

        // Student details are required. There should never be a scenario when `DateTime.MinValue` is used
        // because we check to see if the Student information exists before processing. `DateTime.MinValue`
        // is only for Type requirements
        reportViewModel.DateOfBirth = sessionDTO.Student?.Dob ?? DateTime.MinValue;

        reportViewModel.EnvironmentalRiskFactorFlags = environmentalRiskFactorsFlags.Distinct();
        reportViewModel.SocialRiskFactorFlags = socialRiskFactorFlags
            .Select(x => x.Value).ToList();

        var (assessmentDTO, riskResponseQuestions) = DoAssessmentCalculationsAndPrepareQuestionAnswers(answeredQuestionMap);

        // Easy way to eliminate duplicates
        reportViewModel.Ace = _aces.Distinct();
        reportViewModel.Pce = _pces.Distinct();

        var hasPce = (assessmentDTO.FeProtective > 0 || assessmentDTO.YouProtective > 0);

        // if (assessmentDTO.FeProtective > 0)
        // {
        //     hasPce = true;
        //     reportViewModel.ReportTags.Add(new ReportTag
        //     {
        //         Type = "PCE",
        //         Name = "Family Protective"
        //     });
        // }

        // if (assessmentDTO.YouProtective > 0)
        // {
        //     hasPce = true;
        //     reportViewModel.ReportTags.Add(new ReportTag
        //     {
        //         Type = "PCE",
        //         Name = "Self Protective"
        //     });
        // }

        if (hasPce)
        {
            reportViewModel.ReportTags.Add(new ReportTag
            {
                Type = "PCE",
                Name = "PCE"
            });
        }

        var hasAce = false;
        // if (assessmentDTO.FeRisk > 0)
        // {
        //     hasAce = true;
        //     reportViewModel.ReportTags.Add(new ReportTag
        //     {
        //         Type = "ACE",
        //         Name = "Family Risk"
        //     });
        // }

        if (assessmentDTO.Adhd >= 3)
        {
            hasAce = true;
            reportViewModel.ReportTags.Add(new ReportTag
            {
                Type = "ACE",
                Name = "ADHD"
            });
        }

        if (assessmentDTO.Anxiety > 0)
        {
            hasAce = true;
            reportViewModel.ReportTags.Add(new ReportTag
            {
                Type = "ACE",
                Name = "Anxiety"
            });
        }

        if (assessmentDTO.Depression > 0)
        {
            hasAce = true;
            reportViewModel.ReportTags.Add(new ReportTag
            {
                Type = "ACE",
                Name = "Depression"
            });
        }

        if (assessmentDTO.Suicidality > 0)
        {
            hasAce = true;
            reportViewModel.ReportTags.Add(new ReportTag
            {
                Type = "ACE",
                Name = "Suicidality"
            });
        }

        if (hasAce)
        {
            reportViewModel.ReportTags.Add(new ReportTag
            {
                Type = "ACE",
                Name = "ACE"
            });
        }

        reportViewModel.AnxietyScale = assessmentDTO.Anxiety;
        reportViewModel.DepressionScale = assessmentDTO.Depression;
        // reportViewModel.RiskQuestionResponses = riskResponseQuestions;
        reportViewModel.UserAnswers = riskResponseQuestions.Values.ToList();

        // Note we use the += (compound additive operator) because a FE-Protective score
        // was potentially added above.
        reportViewModel.FeProtectiveTotal += assessmentDTO.FeProtective;
        reportViewModel.FeFeRiskTotal += assessmentDTO.FeRisk;

        reportViewModel.YouProtectiveTotal = assessmentDTO.YouProtective;
        reportViewModel.YouRiskTotal = assessmentDTO.YouRisk;

        reportViewModel.PeopleOverTheAgeOf18 = peopleOverTheAgeOf18;

        return reportViewModel;
    }

    public Tuple<AssessmentDataDTO, Dictionary<Guid, UserAnswerDetailViewModel>> DoAssessmentCalculationsAndPrepareQuestionAnswers(SessionDTO sessionDTO)
    {
        if (sessionDTO is null)
            throw new ArgumentNullException(nameof(sessionDTO));

        var questionMap = sessionDTO.Answers.ToDictionary(x => x.QuestionId);
        return DoAssessmentCalculationsAndPrepareQuestionAnswers(questionMap);
    }

    public Tuple<AssessmentDataDTO, Dictionary<Guid, UserAnswerDetailViewModel>> DoAssessmentCalculationsAndPrepareQuestionAnswers(Dictionary<Guid, AnswerDTO> questionMap)
    {
        AnswerDTO answerDTO;
        var assessmentDataDTO = new AssessmentDataDTO();

        var userAnswers = new Dictionary<Guid, UserAnswerDetailViewModel>();

        // Associated documentation link:
        // https://docs.google.com/spreadsheets/d/1Fzk5a6J58F1OlsCm7eA221gmc-v9T-08/edit#gid=1610914612&range=A2:Q2
        HandleAnswer<AnswerDataLivingSituationDTO>(questionMap, MappedQuestionIds.WhoDoYouLiveWithQuestionId,
            answerPayloadDTO =>
            {
                if (answerPayloadDTO is null or { Skipped: true }) return;

                switch (answerPayloadDTO.Data?.LivingSituation)
                {
                    case LivingSituation.LivesWithBothParents:
                        assessmentDataDTO.FeProtective++;

                        break;

                    case LivingSituation.SplitTimeEvenly:
                    case LivingSituation.SplitTimeMainlyLiveWith:
                    case LivingSituation.LivesWithOneParent:
                    case LivingSituation.LivesWithGrandparents:
                    case LivingSituation.SiblingsAreCareTakers:
                    case LivingSituation.LivesWithSomeoneOtherThanFamily:
                        assessmentDataDTO.FeRisk++;
                        break;

                }

                // Represent rule: "I have one or more other adults that live with me too"
                if (answerPayloadDTO.Data?.GrandParentsLiveAtResidence == true)
                    assessmentDataDTO.FeRisk++;

                if (answerPayloadDTO.Data?.LivingSituation is
                    LivingSituation.SplitTimeEvenly or
                    LivingSituation.SplitTimeMainlyLiveWith or
                    LivingSituation.LivesWithOneParent)
                    assessmentDataDTO.Ace++;
            });

        // if (questionMap.ContainsKey(MappedQuestionIds.WhoDoYouLiveWithQuestionId))
        // {
        //     _logger.LogDebug("Applying rule {QuestionId}", MappedQuestionIds.WhoDoYouLiveWithQuestionId);
        //     
        //     answerDTO = questionMap[MappedQuestionIds.WhoDoYouLiveWithQuestionId];
        //     var answerPayloadDTO = Utility
        //         .DeserializeObject<AnswerPayloadDTO<AnswerDataLivingSituationDTO>>(answerDTO.Data);
        //
        //     if (!answerPayloadDTO.Skipped)
        //     {
        //         switch (answerPayloadDTO.Data?.LivingSituation)
        //         {
        //             case LivingSituation.LivesWithBothParents:
        //                 assessmentDataDTO.FeProtective++;
        //                 
        //                 break;
        //
        //             case LivingSituation.SplitTimeEvenly:
        //             case LivingSituation.SplitTimeMainlyLiveWith:
        //             case LivingSituation.LivesWithOneParent:
        //             case LivingSituation.LivesWithGrandparents:
        //             case LivingSituation.SiblingsAreCareTakers:
        //             case LivingSituation.LivesWithSomeoneOtherThanFamily:
        //                 assessmentDataDTO.FeRisk++;
        //                 break;
        //             
        //         }
        //
        //         // Represent rule: "I have one or more other adults that live with me too"
        //         if (answerPayloadDTO.Data?.GrandParentsLiveAtResidence == true)
        //             assessmentDataDTO.FeRisk++;
        //
        //         if (answerPayloadDTO.Data?.LivingSituation is 
        //             LivingSituation.SplitTimeEvenly or 
        //             LivingSituation.SplitTimeMainlyLiveWith or 
        //             LivingSituation.LivesWithOneParent)
        //             assessmentDataDTO.Ace++;
        //     }
        // }

        // Associated documentation link:
        // https://docs.google.com/spreadsheets/d/1Fzk5a6J58F1OlsCm7eA221gmc-v9T-08/edit#gid=1610914612&range=A8:Q8
        HandleAnswer<IReadOnlyList<AnswerDataParentMood>>(questionMap, MappedQuestionIds.ParentDescribeTheirMoodQuestionId,
            answerPayloadDTO =>
            {
                var characteristicsCollection = answerPayloadDTO
                    ?.Data?.Select(guardian => guardian.Characteristics)
                    .Where(characteristics => !answerPayloadDTO.Skipped && characteristics.Any())
                ?? Enumerable.Empty<List<string>>();

                if (answerPayloadDTO is { Skipped: false, Data: { } })
                    foreach (var characteristics in characteristicsCollection)
                    {
                        var feProtectiveScore = ParentsFeProtectiveIfNotFoundCharacteristics
                            .Except(characteristics)
                            .Count();

                        assessmentDataDTO.FeProtective += feProtectiveScore;

                        // Order of rules defined in the Google Sheet
                        // https://docs.google.com/spreadsheets/d/1Fzk5a6J58F1OlsCm7eA221gmc-v9T-08/edit#gid=95946700
                        if (characteristics.Contains(ParentCharacteristics.Angry)) assessmentDataDTO.FeRisk++;
                        if (characteristics.Contains(ParentCharacteristics.Anxious)) assessmentDataDTO.FeRisk++;

                        if (characteristics.Contains(ParentCharacteristics.Cheerful)) assessmentDataDTO.FeProtective++;
                        if (characteristics.Contains(ParentCharacteristics.Generous)) assessmentDataDTO.FeProtective++;
                        if (characteristics.Contains(ParentCharacteristics.Happy)) assessmentDataDTO.FeProtective++;

                        if (characteristics.Contains(ParentCharacteristics.Irritable)) assessmentDataDTO.FeRisk++;
                        if (characteristics.Contains(ParentCharacteristics.Moody)) assessmentDataDTO.FeRisk++;

                        if (characteristics.Contains(ParentCharacteristics.Optimistic))
                            assessmentDataDTO.FeProtective++;

                        if (characteristics.Contains(ParentCharacteristics.Overwhelmed)) assessmentDataDTO.FeRisk++;

                        if (characteristics.Contains(ParentCharacteristics.Playful)) assessmentDataDTO.FeProtective++;
                        if (characteristics.Contains(ParentCharacteristics.Relaxed)) assessmentDataDTO.FeProtective++;

                        if (characteristics.Contains(ParentCharacteristics.Sad)) assessmentDataDTO.FeRisk++;
                        if (characteristics.Contains(ParentCharacteristics.Tired)) assessmentDataDTO.FeRisk++;

                        // Sub question
                        if (characteristics.Contains(ParentCharacteristics.Listens)) assessmentDataDTO.FeProtective++;


                        if (characteristics.Contains(ParentCharacteristics.HardToConverseWith))
                            assessmentDataDTO.FeRisk++;

                        if (characteristics.Contains(ParentCharacteristics.EatsDinnerTogether))
                            assessmentDataDTO.FeProtective++;

                        if (characteristics.Contains(ParentCharacteristics.Workaholic)) assessmentDataDTO.FeRisk++;

                        if (characteristics.Contains(ParentCharacteristics.SpendTimeTogether))
                            assessmentDataDTO.FeProtective++;

                        if (characteristics.Contains(ParentCharacteristics.Listens) ||
                            characteristics.Contains(ParentCharacteristics.SpendTimeTogether) ||
                            characteristics.Contains(ParentCharacteristics.PositiveFamilyRelationship))
                        {
                            assessmentDataDTO.Pce++;
                        }

                        if (characteristics.Contains(ParentCharacteristics.Distracted)) assessmentDataDTO.FeRisk++;
                        if (characteristics.Contains(ParentCharacteristics.Judgmental)) assessmentDataDTO.FeRisk++;
                        if (characteristics.Contains(ParentCharacteristics.SenseOfHumor))
                            assessmentDataDTO.FeProtective++;

                        if (characteristics.Contains(ParentCharacteristics.CriesOften)) assessmentDataDTO.FeRisk++;
                        if (characteristics.Contains(ParentCharacteristics.ReactsCalmly))
                            assessmentDataDTO.FeProtective++;

                        if (characteristics.Contains(ParentCharacteristics.Critical))
                        {
                            assessmentDataDTO.FeRisk++;
                            assessmentDataDTO.Ace++;
                        }

                        if (characteristics.Contains(ParentCharacteristics.Funny)) assessmentDataDTO.FeProtective++;
                        if (characteristics.Contains(ParentCharacteristics.Sarcastic)) assessmentDataDTO.FeRisk++;

                        if (characteristics.Contains(ParentCharacteristics.Kind)) assessmentDataDTO.FeProtective++;
                        if (characteristics.Contains(ParentCharacteristics.PatientWithOthers))
                            assessmentDataDTO.FeProtective++;

                        if (characteristics.Contains(ParentCharacteristics.Yells)) assessmentDataDTO.FeRisk++;

                        if (characteristics.Contains(ParentCharacteristics.AlcoholicOrDrugUser))
                        {
                            assessmentDataDTO.FeRisk++;
                            assessmentDataDTO.Ace++;
                        }

                        if (characteristics.Contains(ParentCharacteristics.Abstinent)) assessmentDataDTO.FeProtective++;
                        if (characteristics.Contains(ParentCharacteristics.Smokes)) assessmentDataDTO.FeRisk++;
                        if (characteristics.Contains(ParentCharacteristics.Exercises)) assessmentDataDTO.FeProtective++;

                        if (characteristics.Contains(ParentCharacteristics.Swears) &&
                            !characteristics.Contains(ParentCharacteristics.Critical))
                        {
                            assessmentDataDTO.FeRisk++;
                            assessmentDataDTO.Ace++;
                        }

                        if (characteristics.Contains(ParentCharacteristics.Overweight)) assessmentDataDTO.FeRisk++;

                        if (characteristics.Contains(ParentCharacteristics.SickRarely)) assessmentDataDTO.FeProtective++;
                        if (characteristics.Contains(ParentCharacteristics.PlaysSport))
                            assessmentDataDTO.FeProtective++;

                        if (characteristics.Contains(ParentCharacteristics.SleepsOften)) assessmentDataDTO.FeRisk++;
                        if (characteristics.Contains(ParentCharacteristics.LawOffender))
                        {
                            assessmentDataDTO.FeRisk++;
                            assessmentDataDTO.Ace++;
                        }

                        if (characteristics.Contains(ParentCharacteristics.SickOften))
                        {
                            assessmentDataDTO.FeRisk++;
                            // assessmentDataDTO.Ace++;
                        }

                        if (characteristics.Contains(ParentCharacteristics.Volunteers))
                            assessmentDataDTO.FeProtective++;

                        if (characteristics.Contains(ParentCharacteristics.Spiritual)) assessmentDataDTO.FeProtective++;
                        if (characteristics.Contains(ParentCharacteristics.Shares)) assessmentDataDTO.FeProtective++;
                        if (characteristics.Contains(ParentCharacteristics.ParticipatesInCommunity))
                            assessmentDataDTO.FeProtective++;

                        if (characteristics.Contains(ParentCharacteristics.HasFriends))
                            assessmentDataDTO.FeProtective++;

                        if (characteristics.Contains(ParentCharacteristics.OftenInTrouble))
                        {
                            assessmentDataDTO.FeRisk++;
                            assessmentDataDTO.Ace++;
                        }

                        if (characteristics.Contains(ParentCharacteristics.Apologizes))
                            assessmentDataDTO.FeProtective++;

                        if (characteristics.Contains(ParentCharacteristics.PositiveFamilyRelationship))
                        {
                            assessmentDataDTO.FeProtective++;
                        }
                    }
            });

        // Associated documentation link:
        // https://docs.google.com/spreadsheets/d/1Fzk5a6J58F1OlsCm7eA221gmc-v9T-08/edit#gid=1610914612&range=A8:Q8
        // if (questionMap.ContainsKey(MappedQuestionIds.ParentDescribeTheirMoodQuestionId))
        // {
        //     _logger.LogDebug("Applying rule {QuestionId}", MappedQuestionIds.ParentDescribeTheirMoodQuestionId);
        //         
        //     answerDTO = questionMap[MappedQuestionIds.ParentDescribeTheirMoodQuestionId];
        //     var answerPayloadDTO =
        //         Utility.DeserializeObject<AnswerPayloadDTO<IReadOnlyList<AnswerDataParentMood>>>(answerDTO.Data);
        //
        //     // For each risk characteristic not selected "FE-Protective" score is incremented by 1
        //     // This is determined by taking the risk list and finding the negative intersection 
        //     // with the selected characteristics
        //     var characteristicsCollection = answerPayloadDTO
        //         ?.Data?.Select(guardian => guardian.Characteristics)
        //         .Where(characteristics => !answerPayloadDTO.Skipped && characteristics.Any())
        //     ?? Enumerable.Empty<List<string>>();
        //     
        //     if (answerPayloadDTO is { Skipped: false, Data: { }})
        //         foreach (var characteristics in characteristicsCollection)
        //         {
        //             var feProtectiveScore = ParentsFeProtectiveIfNotFoundCharacteristics
        //                 .Except(characteristics)
        //                 .Count();
        //             
        //             assessmentDataDTO.FeProtective += feProtectiveScore;
        //             
        //             // Order of rules defined in the Google Sheet
        //             // https://docs.google.com/spreadsheets/d/1Fzk5a6J58F1OlsCm7eA221gmc-v9T-08/edit#gid=95946700
        //             if (characteristics.Contains(ParentCharacteristics.Angry)) assessmentDataDTO.FeRisk++;
        //             if (characteristics.Contains(ParentCharacteristics.Anxious)) assessmentDataDTO.FeRisk++;
        //
        //             if (characteristics.Contains(ParentCharacteristics.Cheerful)) assessmentDataDTO.FeProtective++;
        //             if (characteristics.Contains(ParentCharacteristics.Generous)) assessmentDataDTO.FeProtective++;
        //             if (characteristics.Contains(ParentCharacteristics.Happy)) assessmentDataDTO.FeProtective++;
        //
        //             if (characteristics.Contains(ParentCharacteristics.Irritable)) assessmentDataDTO.FeRisk++;
        //             if (characteristics.Contains(ParentCharacteristics.Moody)) assessmentDataDTO.FeRisk++;
        //
        //             if (characteristics.Contains(ParentCharacteristics.Optimistic))
        //                 assessmentDataDTO.FeProtective++;
        //
        //             if (characteristics.Contains(ParentCharacteristics.Overwhelmed)) assessmentDataDTO.FeRisk++;
        //
        //             if (characteristics.Contains(ParentCharacteristics.Playful)) assessmentDataDTO.FeProtective++;
        //             if (characteristics.Contains(ParentCharacteristics.Relaxed)) assessmentDataDTO.FeProtective++;
        //
        //             if (characteristics.Contains(ParentCharacteristics.Sad)) assessmentDataDTO.FeRisk++;
        //             if (characteristics.Contains(ParentCharacteristics.Tired)) assessmentDataDTO.FeRisk++;
        //
        //             // Sub question
        //             if (characteristics.Contains(ParentCharacteristics.Listens)) assessmentDataDTO.FeProtective++;
        //             
        //
        //             if (characteristics.Contains(ParentCharacteristics.HardToConverseWith))
        //                 assessmentDataDTO.FeRisk++;
        //             
        //             if (characteristics.Contains(ParentCharacteristics.EatsDinnerTogether))
        //                 assessmentDataDTO.FeProtective++;
        //
        //             if (characteristics.Contains(ParentCharacteristics.Workaholic)) assessmentDataDTO.FeRisk++;
        //             
        //             if (characteristics.Contains(ParentCharacteristics.SpendTimeTogether)) 
        //                 assessmentDataDTO.FeProtective++;
        //
        //             if (characteristics.Contains(ParentCharacteristics.Listens) ||
        //                 characteristics.Contains(ParentCharacteristics.SpendTimeTogether) ||
        //                 characteristics.Contains(ParentCharacteristics.PositiveFamilyRelationship))
        //             {
        //                 assessmentDataDTO.Pce++;
        //             }
        //
        //             if (characteristics.Contains(ParentCharacteristics.Distracted)) assessmentDataDTO.FeRisk++;
        //             if (characteristics.Contains(ParentCharacteristics.Judgmental)) assessmentDataDTO.FeRisk++;
        //             if (characteristics.Contains(ParentCharacteristics.SenseOfHumor))
        //                 assessmentDataDTO.FeProtective++;
        //
        //             if (characteristics.Contains(ParentCharacteristics.CriesOften)) assessmentDataDTO.FeRisk++;
        //             if (characteristics.Contains(ParentCharacteristics.ReactsCalmly))
        //                 assessmentDataDTO.FeProtective++;
        //             
        //             if (characteristics.Contains(ParentCharacteristics.Critical))
        //             {
        //                 assessmentDataDTO.FeRisk++;
        //                 assessmentDataDTO.Ace++;
        //             }
        //
        //             if (characteristics.Contains(ParentCharacteristics.Funny)) assessmentDataDTO.FeProtective++;
        //             if (characteristics.Contains(ParentCharacteristics.Sarcastic)) assessmentDataDTO.FeRisk++;
        //
        //             if (characteristics.Contains(ParentCharacteristics.Kind)) assessmentDataDTO.FeProtective++;
        //             if (characteristics.Contains(ParentCharacteristics.PatientWithOthers))
        //                 assessmentDataDTO.FeProtective++;
        //
        //             if (characteristics.Contains(ParentCharacteristics.Yells)) assessmentDataDTO.FeRisk++;
        //
        //             if (characteristics.Contains(ParentCharacteristics.AlcoholicOrDrugUser))
        //             {
        //                 assessmentDataDTO.FeRisk++;
        //                 assessmentDataDTO.Ace++;
        //             }
        //
        //             if (characteristics.Contains(ParentCharacteristics.Abstinent)) assessmentDataDTO.FeProtective++;
        //             if (characteristics.Contains(ParentCharacteristics.Smokes)) assessmentDataDTO.FeRisk++;
        //             if (characteristics.Contains(ParentCharacteristics.Exercises)) assessmentDataDTO.FeProtective++;
        //
        //             if (characteristics.Contains(ParentCharacteristics.Swears) &&
        //                 !characteristics.Contains(ParentCharacteristics.Critical))
        //             {
        //                 assessmentDataDTO.FeRisk++;
        //                 assessmentDataDTO.Ace++;
        //             }
        //
        //             if (characteristics.Contains(ParentCharacteristics.Overweight)) assessmentDataDTO.FeRisk++;
        //
        //             if (characteristics.Contains(ParentCharacteristics.SickRarely)) assessmentDataDTO.FeProtective++;
        //             if (characteristics.Contains(ParentCharacteristics.PlaysSport))
        //                 assessmentDataDTO.FeProtective++;
        //
        //             if (characteristics.Contains(ParentCharacteristics.SleepsOften)) assessmentDataDTO.FeRisk++;
        //             if (characteristics.Contains(ParentCharacteristics.LawOffender))
        //             {
        //                 assessmentDataDTO.FeRisk++;
        //                 assessmentDataDTO.Ace++;
        //             }
        //
        //             if (characteristics.Contains(ParentCharacteristics.SickOften))
        //             {
        //                 assessmentDataDTO.FeRisk++;
        //                 // assessmentDataDTO.Ace++;
        //             }
        //
        //             if (characteristics.Contains(ParentCharacteristics.Volunteers))
        //                 assessmentDataDTO.FeProtective++;
        //             
        //             if (characteristics.Contains(ParentCharacteristics.Spiritual)) assessmentDataDTO.FeProtective++;
        //             if (characteristics.Contains(ParentCharacteristics.Shares)) assessmentDataDTO.FeProtective++;
        //             if (characteristics.Contains(ParentCharacteristics.ParticipatesInCommunity))
        //                 assessmentDataDTO.FeProtective++;
        //             
        //             if (characteristics.Contains(ParentCharacteristics.HasFriends))
        //                 assessmentDataDTO.FeProtective++;
        //
        //             if (characteristics.Contains(ParentCharacteristics.OftenInTrouble))
        //             {
        //                 assessmentDataDTO.FeRisk++;
        //                 assessmentDataDTO.Ace++;
        //             }
        //
        //             if (characteristics.Contains(ParentCharacteristics.Apologizes))
        //                 assessmentDataDTO.FeProtective++;
        //             
        //             if (characteristics.Contains(ParentCharacteristics.PositiveFamilyRelationship))
        //             {
        //                 assessmentDataDTO.FeProtective++;
        //             }
        //         }
        // }

        // Associated documentation link:
        // https://docs.google.com/spreadsheets/d/1Fzk5a6J58F1OlsCm7eA221gmc-v9T-08/edit#gid=1610914612&range=A58:Q58
        HandleAnswer<IReadOnlyList<AnswerDataSiblingsDTO>>(questionMap, MappedQuestionIds.WhatAboutSiblingsQuestionId,
            answerPayloadDTO =>
            {
                if (answerPayloadDTO is not { Data: { }, Skipped: false }) return;

                var characteristicsCollection = answerPayloadDTO
                    .Data.Select(sibling => sibling.Characteristics ?? new List<string>())
                    .Where(characteristics => characteristics.Any());

                foreach (var characteristics in characteristicsCollection)
                {
                    var feProtectiveScore = SiblingsFeProtectiveNotFoundCharacteristics
                        .Except(characteristics)
                        .Count();

                    assessmentDataDTO.FeProtective += feProtectiveScore;

                    if (characteristics.Contains(SiblingCharacteristics.Happy)) assessmentDataDTO.FeProtective++;
                    if (characteristics.Contains(SiblingCharacteristics.Playful)) assessmentDataDTO.FeProtective++;
                    if (characteristics.Contains(SiblingCharacteristics.Relaxed)) assessmentDataDTO.FeProtective++;

                    if (characteristics.Contains(SiblingCharacteristics.Irritable)) assessmentDataDTO.FeRisk++;
                    if (characteristics.Contains(SiblingCharacteristics.Moody)) assessmentDataDTO.FeRisk++;
                    if (characteristics.Contains(SiblingCharacteristics.InTrouble)) assessmentDataDTO.FeRisk++;
                }
            });

        // Associated documentation link:
        // https://docs.google.com/spreadsheets/d/1Fzk5a6J58F1OlsCm7eA221gmc-v9T-08/edit#gid=1610914612&range=A58:Q58
        // if (questionMap.ContainsKey(MappedQuestionIds.WhatAboutSiblingsQuestionId))
        // {
        //     _logger.LogDebug("Applying rule {QuestionId}", MappedQuestionIds.WhatAboutSiblingsQuestionId);
        //         
        //     answerDTO = questionMap[MappedQuestionIds.WhatAboutSiblingsQuestionId];
        //     var answerPayloadDTO = Utility
        //         .DeserializeObject<AnswerPayloadDTO<IReadOnlyList<AnswerDataSiblingsDTO>>>(answerDTO.Data);
        //
        //     if (answerPayloadDTO.Data != null && !answerPayloadDTO.Skipped)
        //     {
        //         var characteristicsCollection = answerPayloadDTO
        //             .Data.Select(sibling => sibling.Characteristics ?? new List<string>())
        //             .Where(characteristics => characteristics.Any());
        //         
        //         foreach (var characteristics in characteristicsCollection)
        //         {
        //             var feProtectiveScore = SiblingsFeProtectiveNotFoundCharacteristics
        //                 .Except(characteristics)
        //                 .Count();
        //             
        //             assessmentDataDTO.FeProtective += feProtectiveScore;
        //             
        //             if (characteristics.Contains(SiblingCharacteristics.Happy)) assessmentDataDTO.FeProtective++;
        //             if (characteristics.Contains(SiblingCharacteristics.Playful)) assessmentDataDTO.FeProtective++;
        //             if (characteristics.Contains(SiblingCharacteristics.Relaxed)) assessmentDataDTO.FeProtective++;
        //
        //             if (characteristics.Contains(SiblingCharacteristics.Irritable)) assessmentDataDTO.FeRisk++;
        //             if (characteristics.Contains(SiblingCharacteristics.Moody)) assessmentDataDTO.FeRisk++;
        //             if (characteristics.Contains(SiblingCharacteristics.InTrouble)) assessmentDataDTO.FeRisk++;
        //         }
        //     }
        // }

        // Associated documentation link:
        // https://docs.google.com/spreadsheets/d/1Fzk5a6J58F1OlsCm7eA221gmc-v9T-08/edit#gid=1610914612&range=A68:Q68
        HandleAnswer<AnswerDataExperiencedDeathDTO>(questionMap,
            MappedQuestionIds.ExperiencedDeathInTheFamilyQuestionId,
            answerPayloadDTO =>
            {
                if (answerPayloadDTO is null or { Skipped: true }) return;

                var selected = answerPayloadDTO.Data?.Selected;
                switch (selected)
                {
                    case "No":
                        break;

                    case "Yes":
                        assessmentDataDTO.YouRisk++;
                        assessmentDataDTO.Anxiety++;
                        assessmentDataDTO.Depression++;
                        assessmentDataDTO.Internalizing++;

                        // Disabled because of report redundancy
                        // var question = answerDTO.Question?.Title ?? MissingQuestionMessage;
                        // var answer = selected;
                        //
                        // userAnswers.Add(answerDTO.Id, new UserAnswerDetailViewModel
                        // {
                        //     Question = question,
                        //     Answer = answer,
                        //     Flags = new []
                        //     {
                        //         nameof(assessmentDataDTO.Anxiety),
                        //         nameof(assessmentDataDTO.Depression)
                        //     }
                        // });

                        // riskResponseDetailQuestions[nameof(assessmentDataDTO.Anxiety)]
                        //     .Add(new RiskResponseDetailViewModel
                        //     {
                        //         Question = question,
                        //         Answer = "Yes"
                        //     });
                        //
                        // riskResponseDetailQuestions[nameof(assessmentDataDTO.Depression)]
                        //     .Add(new RiskResponseDetailViewModel
                        //     {
                        //         Question = question,
                        //         Answer = "Yes"
                        //     });

                        break;

                    default:
                        throw new Exception($"Selected value {selected} not supported");
                }
            });

        // Associated documentation link:
        // https://docs.google.com/spreadsheets/d/1Fzk5a6J58F1OlsCm7eA221gmc-v9T-08/edit#gid=1610914612&range=A68:Q68        
        // if (questionMap.ContainsKey(MappedQuestionIds.ExperiencedDeathInTheFamilyQuestionId))
        // {
        //     answerDTO = questionMap[MappedQuestionIds.ExperiencedDeathInTheFamilyQuestionId];
        //     var answerPayloadDTO =
        //         Utility.DeserializeObject<AnswerPayloadDTO<AnswerDataExperiencedDeathDTO>>(answerDTO.Data);
        //
        //     if (!answerPayloadDTO.Skipped)
        //     {
        //         var selected = answerPayloadDTO.Data?.Selected;
        //         switch (selected)
        //         {
        //             case "No":
        //                 break;
        //
        //             case "Yes":
        //                 assessmentDataDTO.YouRisk++;
        //                 assessmentDataDTO.Anxiety++;
        //                 assessmentDataDTO.Depression++;
        //                 assessmentDataDTO.Internalizing++;
        //                 
        //                 // Disabled because of report redundancy
        //                 // var question = answerDTO.Question?.Title ?? MissingQuestionMessage;
        //                 // var answer = selected;
        //                 //
        //                 // userAnswers.Add(answerDTO.Id, new UserAnswerDetailViewModel
        //                 // {
        //                 //     Question = question,
        //                 //     Answer = answer,
        //                 //     Flags = new []
        //                 //     {
        //                 //         nameof(assessmentDataDTO.Anxiety),
        //                 //         nameof(assessmentDataDTO.Depression)
        //                 //     }
        //                 // });
        //                 
        //                 // riskResponseDetailQuestions[nameof(assessmentDataDTO.Anxiety)]
        //                 //     .Add(new RiskResponseDetailViewModel
        //                 //     {
        //                 //         Question = question,
        //                 //         Answer = "Yes"
        //                 //     });
        //                 //
        //                 // riskResponseDetailQuestions[nameof(assessmentDataDTO.Depression)]
        //                 //     .Add(new RiskResponseDetailViewModel
        //                 //     {
        //                 //         Question = question,
        //                 //         Answer = "Yes"
        //                 //     });
        //                 
        //                 break;
        //
        //             default:
        //                 throw new Exception($"Selected value {selected} not supported");
        //         }
        //     }
        // }

        // Associated documentation link:
        // https://docs.google.com/spreadsheets/d/1Fzk5a6J58F1OlsCm7eA221gmc-v9T-08/edit#gid=1610914612&range=A71:Q71
        HandleAnswer<AnswerDataSeriouslyIllDTO>(questionMap, MappedQuestionIds.SeriouslyIllFamilyMemberQuestionId,
            answerPayloadDTO =>
            {
                if (answerPayloadDTO is null or { Skipped: true }) return;

                answerDTO = questionMap[MappedQuestionIds.SeriouslyIllFamilyMemberQuestionId];

                var selected = answerPayloadDTO.Data?.Selected;
                switch (selected)
                {
                    case "No":
                        break;

                    case "Yes":
                        assessmentDataDTO.YouRisk++;
                        assessmentDataDTO.Ace++;
                        assessmentDataDTO.Anxiety++;
                        assessmentDataDTO.Depression++;
                        assessmentDataDTO.Internalizing++;

                        var question = answerDTO.Question?.Title ?? MissingQuestionMessage;
                        var answer = selected;

                        userAnswers.Add(answerDTO.QuestionId, new UserAnswerDetailViewModel()
                        {
                            Question = question,
                            Answer = answer,
                            Flags = new[]
                            {
                                nameof(assessmentDataDTO.Ace).ToUpper(),
                                nameof(assessmentDataDTO.Anxiety),
                                nameof(assessmentDataDTO.Depression)
                            }
                        });

                        // riskResponseDetailQuestions[nameof(assessmentDataDTO.Anxiety)]
                        //     .Add(new RiskResponseDetailViewModel
                        //     {
                        //         Question = question,
                        //         Answer = "Yes"
                        //     });
                        //
                        // riskResponseDetailQuestions[nameof(assessmentDataDTO.Depression)]
                        //     .Add(new RiskResponseDetailViewModel
                        //     {
                        //         Question = question,
                        //         Answer = "Yes"
                        //     });


                        _aces.Add(AdverseChildhoodExperiencesDescriptions.LivesWithSeriouslyIllFamilyMember);

                        break;

                    default:
                        throw new Exception($"Selected value {selected} not supported");
                }
            });

        // Associated documentation link:
        // https://docs.google.com/spreadsheets/d/1Fzk5a6J58F1OlsCm7eA221gmc-v9T-08/edit#gid=1610914612&range=A71:Q71
        // if (questionMap.ContainsKey(MappedQuestionIds.SeriouslyIllFamilyMemberQuestionId))
        // {
        //     _logger.LogDebug("Applying rule {QuestionId}", MappedQuestionIds.SeriouslyIllFamilyMemberQuestionId);
        //         
        //     answerDTO = questionMap[MappedQuestionIds.SeriouslyIllFamilyMemberQuestionId];
        //     var answerPayloadDTO =
        //         Utility.DeserializeObject<AnswerPayloadDTO<AnswerDataSeriouslyIllDTO>>(answerDTO.Data);
        //
        //     if (!answerPayloadDTO.Skipped)
        //     {
        //         var selected = answerPayloadDTO.Data?.Selected;
        //         switch (selected)
        //         {
        //             case "No":
        //                 break;
        //
        //             case "Yes":
        //                 assessmentDataDTO.YouRisk++;
        //                 assessmentDataDTO.Ace++;
        //                 assessmentDataDTO.Anxiety++;
        //                 assessmentDataDTO.Depression++;
        //                 assessmentDataDTO.Internalizing++;
        //                 
        //                 var question = answerDTO.Question?.Title ?? MissingQuestionMessage;
        //                 var answer = selected;
        //
        //                 userAnswers.Add(answerDTO.QuestionId, new UserAnswerDetailViewModel()
        //                 {
        //                     Question = question,
        //                     Answer = answer,
        //                     Flags = new []
        //                     {
        //                         nameof(assessmentDataDTO.Ace).ToUpper(),
        //                         nameof(assessmentDataDTO.Anxiety),
        //                         nameof(assessmentDataDTO.Depression)
        //                     }
        //                 });
        //                 
        //                 // riskResponseDetailQuestions[nameof(assessmentDataDTO.Anxiety)]
        //                 //     .Add(new RiskResponseDetailViewModel
        //                 //     {
        //                 //         Question = question,
        //                 //         Answer = "Yes"
        //                 //     });
        //                 //
        //                 // riskResponseDetailQuestions[nameof(assessmentDataDTO.Depression)]
        //                 //     .Add(new RiskResponseDetailViewModel
        //                 //     {
        //                 //         Question = question,
        //                 //         Answer = "Yes"
        //                 //     });
        //                 
        //                 
        //                 _aces.Add(AdverseChildhoodExperiencesDescriptions.LivesWithSeriouslyIllFamilyMember);
        //                     
        //                 break;
        //
        //             default:
        //                 throw new Exception($"Selected value {selected} not supported");
        //         }
        //     }
        // }

        // Associated documentation link:
        // https://docs.google.com/spreadsheets/d/1Fzk5a6J58F1OlsCm7eA221gmc-v9T-08/edit#gid=1610914612&range=A75:Q75
        HandleAnswer<IReadOnlyList<string>>(questionMap, MappedQuestionIds.WhoCanYouCountOnInTheFamilyQuestionId,
            answerPayloadDTO =>
            {
                var members = answerPayloadDTO?.Data;

                if (members is null) return;

                var family = new[]
                {
                    "Maternal Grandmother",
                    "Maternal Grandfather",
                    "Paternal Grandmother",
                    "Paternal Grandfather",
                    "Step Father",
                    "Mother",
                    "Father",
                    "Step Mother",
                    "Step Sister",
                    "Brother"
                };

                if (members.Any(x => family.Contains(x)))
                {
                    assessmentDataDTO.FeProtective++;
                    assessmentDataDTO.Pce++;
                }
            });

        // Associated documentation link:
        // https://docs.google.com/spreadsheets/d/1Fzk5a6J58F1OlsCm7eA221gmc-v9T-08/edit#gid=1610914612&range=A75:Q75
        // if (questionMap.ContainsKey(MappedQuestionIds.WhoCanYouCountOnInTheFamilyQuestionId))
        // {
        //     _logger.LogDebug("Applying rule {QuestionId}", MappedQuestionIds.WhoCanYouCountOnInTheFamilyQuestionId);
        //         
        //     answerDTO = questionMap[MappedQuestionIds.WhoCanYouCountOnInTheFamilyQuestionId];
        //     var answerPayloadDTO = Utility.DeserializeObject<AnswerPayloadDTO<IReadOnlyList<string>>>(answerDTO.Data);
        //     var members = answerPayloadDTO?.Data;
        //
        //     if (members is not null)
        //     {
        //         var family = new[]
        //         {
        //             "Maternal Grandmother",
        //             "Maternal Grandfather",
        //             "Paternal Grandmother",
        //             "Paternal Grandfather",
        //             "Step Father",
        //             "Mother",
        //             "Father",
        //             "Step Mother",
        //             "Step Sister",
        //             "Brother"
        //         };
        //
        //         if (members.Any(x => family.Contains(x)))
        //         {
        //             assessmentDataDTO.FeProtective++;
        //             assessmentDataDTO.Pce++;
        //         }
        //     }
        // }

        // Associated documentation link:
        // https://docs.google.com/spreadsheets/d/1Fzk5a6J58F1OlsCm7eA221gmc-v9T-08/edit#gid=1610914612&range=A89:Q89
        HandleAnswer<List<AnswerDataOutsideFamilyCanCountOnDTO>>(questionMap,
            MappedQuestionIds.AreTherePeopleOutsideYouCanCountOnQuestionId,
            answerPayloadDTO =>
            {
                if (answerPayloadDTO is null) return;

                var peopleList = answerPayloadDTO.Data ?? new List<AnswerDataOutsideFamilyCanCountOnDTO>();

                if (answerPayloadDTO is { Skipped: false })
                {
                    if (peopleList.Any())
                        assessmentDataDTO.YouProtective++;

                    // If there's more than 2 people over the age of 18.
                    if (peopleList.Count(x => x.OverAge18 == true) >= 2)
                        assessmentDataDTO.Pce++;
                }

                if (answerPayloadDTO is { Skipped: true } || peopleList.Count == 0)
                {
                    assessmentDataDTO.Internalizing++;
                    assessmentDataDTO.Externalizing++;
                }
            });

        // Associated documentation link:
        // https://docs.google.com/spreadsheets/d/1Fzk5a6J58F1OlsCm7eA221gmc-v9T-08/edit#gid=1610914612&range=A89:Q89
        // if (questionMap.ContainsKey(MappedQuestionIds.AreTherePeopleOutsideYouCanCountOnQuestionId))
        // {
        //     _logger.LogDebug("Applying rule {QuestionId}",
        //         MappedQuestionIds.AreTherePeopleOutsideYouCanCountOnQuestionId);
        //         
        //     answerDTO = questionMap[MappedQuestionIds.AreTherePeopleOutsideYouCanCountOnQuestionId];
        //     var answerPayloadDTO = Utility
        //         .DeserializeObject<AnswerPayloadDTO<List<AnswerDataOutsideFamilyCanCountOnDTO>>>(answerDTO.Data);
        //
        //     var peopleList = answerPayloadDTO.Data ?? new List<AnswerDataOutsideFamilyCanCountOnDTO>();
        //     
        //     if (!answerPayloadDTO.Skipped)
        //     {
        //         if (peopleList.Any())
        //             assessmentDataDTO.YouProtective++;
        //
        //         // If there's more than 2 people over the age of 18.
        //         if (peopleList.Count(x => x.OverAge18 == true) >= 2)
        //             assessmentDataDTO.Pce++;
        //     }
        //     
        //     if (answerPayloadDTO.Skipped || peopleList.Count == 0)
        //     {
        //         assessmentDataDTO.Internalizing++;
        //         assessmentDataDTO.Externalizing++;
        //     }
        // }

        // Associated documentation link:
        // https://docs.google.com/spreadsheets/d/1Fzk5a6J58F1OlsCm7eA221gmc-v9T-08/edit#gid=1610914612&range=A92:Q92
        HandleAnswer<int?>(questionMap, MappedQuestionIds.RaceSkinColorReligionQuestionId, answerPayloadDTO =>
        {
            if (answerPayloadDTO is null or { Skipped: true }) return;

            var scale = answerPayloadDTO.Data;

            if (scale is null) return;

            answerDTO = questionMap[MappedQuestionIds.RaceSkinColorReligionQuestionId];
            if (answerDTO.Question?.Type != "Scale")
                throw new Exception("Expecting answer to belong to a question type of Scale");

            switch (scale)
            {
                case >= 4:
                    assessmentDataDTO.YouRisk++;
                    assessmentDataDTO.Anxiety++;

                    var question = answerDTO.Question?.Title ?? MissingQuestionMessage;
                    var answer = scale.ToString() ?? MissingAnswerMessage;

                    userAnswers.Add(answerDTO.QuestionId, new UserAnswerDetailViewModel()
                    {
                        Question = question,
                        Answer = answer,
                        Flags = new[] { nameof(assessmentDataDTO.Anxiety) }
                    });

                    break;

                case <= 2:
                    assessmentDataDTO.YouProtective++;
                    break;
            }

            assessmentDataDTO.Internalizing++;
            assessmentDataDTO.Externalizing++;
        });

        // Associated documentation link:
        // https://docs.google.com/spreadsheets/d/1Fzk5a6J58F1OlsCm7eA221gmc-v9T-08/edit#gid=1610914612&range=A92:Q92
        // if (questionMap.ContainsKey(MappedQuestionIds.RaceSkinColorReligionQuestionId))
        // {
        //     _logger.LogDebug("Applying rule {QuestionId}", MappedQuestionIds.RaceSkinColorReligionQuestionId);
        //         
        //     answerDTO = questionMap[MappedQuestionIds.RaceSkinColorReligionQuestionId];
        //     var answerPayloadDTO = Utility.DeserializeObject<AnswerPayloadDTO<int?>>(answerDTO.Data);
        //     var scale = answerPayloadDTO.Data;
        //
        //     if (answerDTO.Question?.Type != "Scale")
        //         throw new Exception("Expecting answer to belong to a question type of Scale");
        //
        //     if (!answerPayloadDTO.Skipped && scale is not null)
        //     {
        //         switch (scale)
        //         {
        //             case >= 4:
        //                 assessmentDataDTO.YouRisk++;
        //                 assessmentDataDTO.Anxiety++;
        //                 
        //                 var question = answerDTO.Question?.Title ?? MissingQuestionMessage;
        //                 var answer = scale.ToString() ?? MissingAnswerMessage;
        //                 
        //                 userAnswers.Add(answerDTO.QuestionId, new UserAnswerDetailViewModel()
        //                 {
        //                     Question = question,
        //                     Answer = answer,
        //                     Flags = new [] { nameof(assessmentDataDTO.Anxiety) }
        //                 });
        //
        //                 break;
        //
        //             case <= 2:
        //                 assessmentDataDTO.YouProtective++;
        //                 break;
        //         }
        //
        //         assessmentDataDTO.Internalizing++;
        //         assessmentDataDTO.Externalizing++;
        //     }
        // }

        // Associated documentation link:
        // https://docs.google.com/spreadsheets/d/1Fzk5a6J58F1OlsCm7eA221gmc-v9T-08/edit#gid=1610914612&range=A93:Q93
        HandleAnswer<bool?>(questionMap, MappedQuestionIds.SeeingMentalHealthProfessionallyQuestionId,
            answerPayloadDTO =>
            {
                if (answerPayloadDTO is null or { Skipped: true }) return;

                var result = answerPayloadDTO.Data;

                if (result == true)
                {
                    assessmentDataDTO.YouProtective++;
                }
            });

        // Associated documentation link:
        // https://docs.google.com/spreadsheets/d/1Fzk5a6J58F1OlsCm7eA221gmc-v9T-08/edit#gid=1610914612&range=A93:Q93
        // if (questionMap.ContainsKey(MappedQuestionIds.SeeingMentalHealthProfessionallyQuestionId))
        // {
        //     _logger.LogDebug("Applying rule {QuestionId}",
        //         MappedQuestionIds.SeeingMentalHealthProfessionallyQuestionId);
        //         
        //     answerDTO = questionMap[MappedQuestionIds.SeeingMentalHealthProfessionallyQuestionId];
        //     var answerPayloadDTO = Utility.DeserializeObject<AnswerPayloadDTO<bool?>>(answerDTO.Data);
        //     var result = answerPayloadDTO.Data;
        //
        //     if (!answerPayloadDTO.Skipped && result is not null && result == true)
        //     {
        //         assessmentDataDTO.YouProtective++;
        //     }
        // }

        // Associated documentation link:
        // https://docs.google.com/spreadsheets/d/1Fzk5a6J58F1OlsCm7eA221gmc-v9T-08/edit#gid=1610914612&range=A96:Q96
        HandleAnswer<int?>(questionMap, MappedQuestionIds.HappyOverallQuestionId, answerPayloadDTO =>
        {
            if (answerPayloadDTO is null or { Skipped: true }) return;

            answerDTO = questionMap[MappedQuestionIds.HappyOverallQuestionId];

            var scale = answerPayloadDTO?.Data;

            if (scale is null) return;

            if (answerDTO.Question?.Type != "Scale")
                throw new Exception("Expecting answer to belong to a question type of Scale");

            switch (scale)
            {
                case <= 2:
                    assessmentDataDTO.YouRisk++;
                    assessmentDataDTO.Anxiety++;
                    assessmentDataDTO.Depression++;
                    assessmentDataDTO.Internalizing++;

                    var question = answerDTO.Question?.Title ?? MissingQuestionMessage;
                    var answer = scale.ToString() ?? MissingAnswerMessage;

                    userAnswers.Add(answerDTO.QuestionId, new UserAnswerDetailViewModel
                    {
                        Question = question,
                        Answer = answer,
                        Flags = new[]
                        {
                            nameof(assessmentDataDTO.Anxiety),
                            nameof(assessmentDataDTO.Depression)
                        }
                    });

                    break;
                case >= 4:
                    assessmentDataDTO.YouProtective++;
                    break;
            }
        });

        // Associated documentation link:
        // https://docs.google.com/spreadsheets/d/1Fzk5a6J58F1OlsCm7eA221gmc-v9T-08/edit#gid=1610914612&range=A96:Q96
        // if (questionMap.ContainsKey(MappedQuestionIds.HappyOverallQuestionId))
        // {
        //     _logger.LogDebug("Applying rule {QuestionId}", MappedQuestionIds.HappyOverallQuestionId);
        //     
        //     answerDTO = questionMap[MappedQuestionIds.HappyOverallQuestionId];
        //     var answerPayloadDTO = Utility.DeserializeObject<AnswerPayloadDTO<int?>>(answerDTO.Data);
        //     var scale = answerPayloadDTO.Data;
        //
        //     if (answerDTO.Question?.Type != "Scale")
        //         throw new Exception("Expecting answer to belong to a question type of Scale");
        //
        //     if (!answerPayloadDTO.Skipped && scale is not null)
        //     {
        //         switch (scale)
        //         {
        //             case <= 2:
        //                 assessmentDataDTO.YouRisk++;
        //                 assessmentDataDTO.Anxiety++;
        //                 assessmentDataDTO.Depression++;
        //                 assessmentDataDTO.Internalizing++;
        //                 
        //                 var question = answerDTO.Question?.Title ?? MissingQuestionMessage;
        //                 var answer = scale.ToString() ?? MissingAnswerMessage;
        //                 
        //                 userAnswers.Add(answerDTO.QuestionId, new UserAnswerDetailViewModel
        //                 {
        //                     Question = question,
        //                     Answer = answer,
        //                     Flags = new []
        //                     {
        //                         nameof(assessmentDataDTO.Anxiety), 
        //                         nameof(assessmentDataDTO.Depression)
        //                     }
        //                 });
        //                 
        //                 break;
        //             case >= 4:
        //                 assessmentDataDTO.YouProtective++;
        //                 break;
        //         }
        //     }
        // }

        HandleAnswer<string?>(questionMap, MappedQuestionIds.WhatMakesYouHappyQuestionId, answerPayloadDTO =>
        {
            if (answerPayloadDTO is null or { Skipped: true } || string.IsNullOrWhiteSpace(answerPayloadDTO.Data))
                return;

            answerDTO = questionMap[MappedQuestionIds.WhatMakesYouHappyQuestionId];

            var question = answerDTO.Question?.Title ?? MissingQuestionMessage;

            userAnswers.Add(answerDTO.QuestionId, new UserAnswerDetailViewModel
            {
                Question = question,
                Answer = answerPayloadDTO.Data,
                Flags = Enumerable.Empty<string>()
            });
        });

        // if (questionMap.ContainsKey(MappedQuestionIds.WhatMakesYouHappyQuestionId))
        // {
        //     _logger.LogDebug("Applying rule {QuestionId}", MappedQuestionIds.WhatMakesYouHappyQuestionId);
        //         
        //     answerDTO = questionMap[MappedQuestionIds.WhatMakesYouHappyQuestionId];
        //     var answerPayloadDTO = Utility.DeserializeObject<AnswerPayloadDTO<string?>>(answerDTO.Data);
        //
        //     if (answerPayloadDTO is { Skipped: false } && !string.IsNullOrWhiteSpace(answerPayloadDTO.Data))
        //     {
        //         var question = answerDTO.Question?.Title ?? MissingQuestionMessage;
        //         
        //         userAnswers.Add(answerDTO.QuestionId, new UserAnswerDetailViewModel
        //         {
        //             Question = question,
        //             Answer = answerPayloadDTO.Data,
        //             Flags = Enumerable.Empty<string>()
        //         });   
        //     }
        // }

        HandleAnswer<string?>(questionMap, MappedQuestionIds.WhatCouldMakeYouHappierQuestionId, answerPayloadDTO =>
        {
            if (answerPayloadDTO is null or { Skipped: true } || string.IsNullOrWhiteSpace(answerPayloadDTO.Data))
                return;

            answerDTO = questionMap[MappedQuestionIds.WhatCouldMakeYouHappierQuestionId];

            var question = answerDTO.Question?.Title ?? MissingQuestionMessage;

            userAnswers.Add(answerDTO.QuestionId, new UserAnswerDetailViewModel
            {
                Question = question,
                Answer = answerPayloadDTO.Data,
                Flags = Enumerable.Empty<string>()
            });
        });

        // if (questionMap.ContainsKey(MappedQuestionIds.WhatCouldMakeYouHappierQuestionId))
        // {
        //     _logger.LogDebug("Applying rule {QuestionId}", MappedQuestionIds.WhatCouldMakeYouHappierQuestionId);
        //         
        //     answerDTO = questionMap[MappedQuestionIds.WhatCouldMakeYouHappierQuestionId];
        //     var answerPayloadDTO = Utility.DeserializeObject<AnswerPayloadDTO<string?>>(answerDTO.Data);
        //
        //     if (answerPayloadDTO is { Skipped: false } && !string.IsNullOrWhiteSpace(answerPayloadDTO.Data))
        //     {
        //         var question = answerDTO.Question?.Title ?? MissingQuestionMessage;
        //         
        //         userAnswers.Add(answerDTO.QuestionId, new UserAnswerDetailViewModel
        //         {
        //             Question = question,
        //             Answer = answerPayloadDTO.Data,
        //             Flags = Enumerable.Empty<string>()
        //         });
        //     }
        // }

        // Associated documentation link:
        // https://docs.google.com/spreadsheets/d/1Fzk5a6J58F1OlsCm7eA221gmc-v9T-08/edit#gid=1610914612&range=A100:Q100
        HandleAnswer<int?>(questionMap, MappedQuestionIds.HowTiredDuringSchoolDayQuestionId, answerPayloadDTO =>
        {
            if (answerPayloadDTO is null or { Skipped: true }) return;

            answerDTO = questionMap[MappedQuestionIds.HowTiredDuringSchoolDayQuestionId];

            var scale = answerPayloadDTO.Data;
            if (scale == null) return;

            switch (scale)
            {
                case >= 4:
                    assessmentDataDTO.YouRisk++;
                    assessmentDataDTO.Anxiety++;
                    assessmentDataDTO.Depression++;
                    assessmentDataDTO.Internalizing++;

                    var question = answerDTO.Question?.Title ?? MissingQuestionMessage;
                    var answer = scale.ToString() ?? MissingAnswerMessage;

                    userAnswers.Add(answerDTO.QuestionId, new UserAnswerDetailViewModel
                    {
                        Question = question,
                        Answer = answer,
                        Flags = new[]
                        {
                            nameof(assessmentDataDTO.Anxiety),
                            nameof(assessmentDataDTO.Depression)
                        }
                    });

                    break;

                case <= 2:
                    assessmentDataDTO.YouProtective++;
                    break;
            }
        });

        // Associated documentation link:
        // https://docs.google.com/spreadsheets/d/1Fzk5a6J58F1OlsCm7eA221gmc-v9T-08/edit#gid=1610914612&range=A100:Q100
        // if (questionMap.ContainsKey(MappedQuestionIds.HowTiredDuringSchoolDayQuestionId))
        // {
        //     _logger.LogDebug("Applying rule {QuestionId}", MappedQuestionIds.HowTiredDuringSchoolDayQuestionId);
        //         
        //     answerDTO = questionMap[MappedQuestionIds.HowTiredDuringSchoolDayQuestionId];
        //     var answerPayloadDTO = Utility.DeserializeObject<AnswerPayloadDTO<int?>>(answerDTO.Data);
        //     var scale = answerPayloadDTO.Data;
        //
        //     if (!answerPayloadDTO.Skipped && scale is not null)
        //     {
        //         switch (scale)
        //         {
        //             case >= 4:
        //                 assessmentDataDTO.YouRisk++;
        //                 assessmentDataDTO.Anxiety++;
        //                 assessmentDataDTO.Depression++;
        //                 assessmentDataDTO.Internalizing++;
        //                 
        //                 var question = answerDTO.Question?.Title ?? MissingQuestionMessage;
        //                 var answer = scale.ToString() ?? MissingAnswerMessage;
        //                 
        //                 userAnswers.Add(answerDTO.QuestionId, new UserAnswerDetailViewModel
        //                 {
        //                     Question = question,
        //                     Answer = answer,
        //                     Flags = new []
        //                     {
        //                         nameof(assessmentDataDTO.Anxiety),
        //                         nameof(assessmentDataDTO.Depression)
        //                     }
        //                 });
        //
        //                 break;
        //
        //             case <= 2:
        //                 assessmentDataDTO.YouProtective++;
        //                 break;
        //         }
        //     }
        // }

        HandleAnswer<string?>(questionMap, MappedQuestionIds.WhatMakesYouTiredQuestionId, answerPayloadDTO =>
        {
            if (answerPayloadDTO is null or { Skipped: true } || string.IsNullOrWhiteSpace(answerPayloadDTO.Data))
                return;

            answerDTO = questionMap[MappedQuestionIds.WhatMakesYouTiredQuestionId];

            var question = answerDTO.Question?.Title ?? MissingQuestionMessage;

            userAnswers.Add(answerDTO.QuestionId, new UserAnswerDetailViewModel
            {
                Question = question,
                Answer = answerPayloadDTO.Data,
                Flags = Enumerable.Empty<string>()
            });
        });

        // if (questionMap.ContainsKey(MappedQuestionIds.WhatMakesYouTiredQuestionId))
        // {
        //     _logger.LogDebug("Applying rule {QuestionId}", MappedQuestionIds.WhatMakesYouTiredQuestionId);
        //         
        //     answerDTO = questionMap[MappedQuestionIds.WhatMakesYouTiredQuestionId];
        //     var answerPayloadDTO = Utility.DeserializeObject<AnswerPayloadDTO<string?>>(answerDTO.Data);
        //
        //     if (answerPayloadDTO is { Skipped: false } && !string.IsNullOrWhiteSpace(answerPayloadDTO.Data))
        //     {
        //         var question = answerDTO.Question?.Title ?? MissingQuestionMessage;
        //         
        //         userAnswers.Add(answerDTO.QuestionId, new UserAnswerDetailViewModel
        //         {
        //             Question = question,
        //             Answer = answerPayloadDTO.Data,
        //             Flags = Enumerable.Empty<string>()
        //         });
        //     }
        // }

        HandleAnswer<string?>(questionMap, MappedQuestionIds.WhatMakesYouLessTiredQuestionId, answerPayloadDTO =>
        {
            if (answerPayloadDTO is null or { Skipped: true } || string.IsNullOrWhiteSpace(answerPayloadDTO.Data))
                return;

            answerDTO = questionMap[MappedQuestionIds.WhatMakesYouLessTiredQuestionId];

            var question = answerDTO.Question?.Title ?? MissingQuestionMessage;

            userAnswers.Add(answerDTO.QuestionId, new UserAnswerDetailViewModel
            {
                Question = question,
                Answer = answerPayloadDTO.Data,
                Flags = Enumerable.Empty<string>()
            });
        });

        // if (questionMap.ContainsKey(MappedQuestionIds.WhatMakesYouLessTiredQuestionId))
        // {
        //     _logger.LogDebug("Applying rule {QuestionId}", MappedQuestionIds.WhatMakesYouLessTiredQuestionId);
        //         
        //     answerDTO = questionMap[MappedQuestionIds.WhatMakesYouLessTiredQuestionId];
        //     var answerPayloadDTO = Utility.DeserializeObject<AnswerPayloadDTO<string?>>(answerDTO.Data);
        //
        //     if (answerPayloadDTO is { Skipped: false } && !string.IsNullOrWhiteSpace(answerPayloadDTO.Data))
        //     {
        //         var question = answerDTO.Question?.Title ?? MissingQuestionMessage;
        //         
        //         userAnswers.Add(answerDTO.QuestionId, new UserAnswerDetailViewModel
        //         {
        //             Question = question,
        //             Answer = answerPayloadDTO.Data,
        //             Flags = Enumerable.Empty<string>()
        //         });
        //     }
        // }

        // Associated documentation link:
        // https://docs.google.com/spreadsheets/d/1Fzk5a6J58F1OlsCm7eA221gmc-v9T-08/edit#gid=1610914612&range=A103:Q103
        HandleAnswer<int?>(questionMap, MappedQuestionIds.HowAreYouDoingAcademicallyQuestionId, answerPayloadDTO =>
        {
            if (answerPayloadDTO is null or { Skipped: true }) return;

            answerDTO = questionMap[MappedQuestionIds.HowAreYouDoingAcademicallyQuestionId];

            var scale = answerPayloadDTO.Data;
            if (scale is null) return;

            switch (scale)
            {
                case >= 4:
                    assessmentDataDTO.YouProtective++;
                    break;

                case <= 2:
                    assessmentDataDTO.YouRisk++;
                    assessmentDataDTO.Anxiety++;
                    assessmentDataDTO.Depression++;

                    var question = answerDTO.Question?.Title ?? MissingQuestionMessage;
                    var answer = scale.ToString() ?? MissingAnswerMessage;

                    userAnswers.Add(answerDTO.QuestionId, new UserAnswerDetailViewModel
                    {
                        Question = question,
                        Answer = answer,
                        Flags = new[]
                        {
                            nameof(assessmentDataDTO.Anxiety),
                            nameof(assessmentDataDTO.Depression)
                        }
                    });

                    break;
            }
        });

        // Associated documentation link:
        // https://docs.google.com/spreadsheets/d/1Fzk5a6J58F1OlsCm7eA221gmc-v9T-08/edit#gid=1610914612&range=A103:Q103
        // if (questionMap.ContainsKey(MappedQuestionIds.HowAreYouDoingAcademicallyQuestionId))
        // {
        //     _logger.LogDebug("Applying rule {QuestionId}", MappedQuestionIds.HowAreYouDoingAcademicallyQuestionId);
        //         
        //     answerDTO = questionMap[MappedQuestionIds.HowAreYouDoingAcademicallyQuestionId];
        //     var answerPayloadDTO = Utility.DeserializeObject<AnswerPayloadDTO<int?>>(answerDTO.Data);
        //     var scale = answerPayloadDTO.Data;
        //
        //     if (!answerPayloadDTO.Skipped && scale is not null)
        //     {
        //         switch (scale)
        //         {
        //             case >= 4:
        //                 assessmentDataDTO.YouProtective++;
        //                 break;
        //
        //             case <= 2:
        //                 assessmentDataDTO.YouRisk++;
        //                 assessmentDataDTO.Anxiety++;
        //                 assessmentDataDTO.Depression++;
        //                 
        //                 var question = answerDTO.Question?.Title ?? MissingQuestionMessage;
        //                 var answer = scale.ToString() ?? MissingAnswerMessage;
        //                 
        //                 userAnswers.Add(answerDTO.QuestionId, new UserAnswerDetailViewModel
        //                 {
        //                     Question = question,
        //                     Answer = answer,
        //                     Flags = new []
        //                     {
        //                         nameof(assessmentDataDTO.Anxiety),
        //                         nameof(assessmentDataDTO.Depression)
        //                     }
        //                 });
        //
        //                 break;
        //         }
        //     }
        // }

        HandleAnswer<string?>(questionMap, MappedQuestionIds.HowCanYouImproveAcademicallyQuestionId, answerPayloadDTO =>
        {
            if (answerPayloadDTO is null or { Skipped: true } || string.IsNullOrWhiteSpace(answerPayloadDTO.Data))
                return;

            answerDTO = questionMap[MappedQuestionIds.HowCanYouImproveAcademicallyQuestionId];

            var question = answerDTO.Question?.Title ?? MissingQuestionMessage;

            userAnswers.Add(answerDTO.QuestionId, new UserAnswerDetailViewModel
            {
                Question = question,
                Answer = answerPayloadDTO.Data,
                Flags = Enumerable.Empty<string>()
            });
        });

        // if (questionMap.ContainsKey(MappedQuestionIds.HowCanYouImproveAcademicallyQuestionId))
        // {
        //     _logger.LogDebug("Applying rule {QuestionId}", MappedQuestionIds.HowCanYouImproveAcademicallyQuestionId);
        //     
        //     answerDTO = questionMap[MappedQuestionIds.HowCanYouImproveAcademicallyQuestionId];
        //     var answerPayloadDTO = Utility.DeserializeObject<AnswerPayloadDTO<string?>>(answerDTO.Data);
        //
        //     if (answerPayloadDTO is { Skipped: false } && !string.IsNullOrWhiteSpace(answerPayloadDTO.Data))
        //     {
        //         var question = answerDTO.Question?.Title ?? MissingQuestionMessage;
        //         
        //         userAnswers.Add(answerDTO.QuestionId, new UserAnswerDetailViewModel
        //         {
        //             Question = question,
        //             Answer = answerPayloadDTO.Data,
        //             Flags = Enumerable.Empty<string>()
        //         });
        //     }
        // }

        // Associated documentation link:
        // https://docs.google.com/spreadsheets/d/1Fzk5a6J58F1OlsCm7eA221gmc-v9T-08/edit#gid=1610914612&range=A104:Q104
        HandleAnswer<int?>(questionMap, MappedQuestionIds.HowAreYouDoingSociallyQuestionId, answerPayloadDTO =>
        {
            if (answerPayloadDTO is null or { Skipped: true }) return;

            answerDTO = questionMap[MappedQuestionIds.HowAreYouDoingSociallyQuestionId];

            var scale = answerPayloadDTO.Data;
            if (scale is null) return;

            switch (scale)
            {
                case >= 4:
                    assessmentDataDTO.YouProtective++;
                    break;

                case <= 2:
                    assessmentDataDTO.YouRisk++;
                    assessmentDataDTO.Anxiety++;
                    assessmentDataDTO.Depression++;

                    var question = answerDTO.Question?.Title ?? MissingQuestionMessage;
                    var answer = scale.ToString() ?? MissingAnswerMessage;

                    userAnswers.Add(answerDTO.QuestionId, new UserAnswerDetailViewModel
                    {
                        Question = question,
                        Answer = answer,
                        Flags = new[]
                        {
                            nameof(assessmentDataDTO.Anxiety),
                            nameof(assessmentDataDTO.Depression)
                        }
                    });

                    break;
            }
        });

        // Associated documentation link:
        // https://docs.google.com/spreadsheets/d/1Fzk5a6J58F1OlsCm7eA221gmc-v9T-08/edit#gid=1610914612&range=A104:Q104
        // if (questionMap.ContainsKey(MappedQuestionIds.HowAreYouDoingSociallyQuestionId))
        // {
        //     _logger.LogDebug("Applying rule {QuestionId}", MappedQuestionIds.HowAreYouDoingSociallyQuestionId);
        //     
        //     answerDTO = questionMap[MappedQuestionIds.HowAreYouDoingSociallyQuestionId];
        //     var answerPayloadDTO = Utility.DeserializeObject<AnswerPayloadDTO<int?>>(answerDTO.Data);
        //     var scale = answerPayloadDTO.Data;
        //
        //     if (!answerPayloadDTO.Skipped && scale is not null)
        //     {
        //         switch (scale)
        //         {
        //             case >= 4:
        //                 assessmentDataDTO.YouProtective++;
        //                 break;
        //
        //             case <= 2:
        //                 assessmentDataDTO.YouRisk++;
        //                 assessmentDataDTO.Anxiety++;
        //                 assessmentDataDTO.Depression++;
        //                 
        //                 var question = answerDTO.Question?.Title ?? MissingQuestionMessage;
        //                 var answer = scale.ToString() ?? MissingAnswerMessage;
        //                 
        //                 userAnswers.Add(answerDTO.QuestionId, new UserAnswerDetailViewModel
        //                 {
        //                     Question = question,
        //                     Answer = answer,
        //                     Flags = new []
        //                     {
        //                         nameof(assessmentDataDTO.Anxiety),
        //                         nameof(assessmentDataDTO.Depression)
        //                     }
        //                 });
        //
        //                 break;
        //         }
        //     }
        // }

        HandleAnswer<string?>(questionMap, MappedQuestionIds.OneThingCouldBeDoneToImproveSocialLife, answerPayloadDTO =>
        {
            if (answerPayloadDTO is null or { Skipped: true } || string.IsNullOrWhiteSpace(answerPayloadDTO.Data))
                return;

            answerDTO = questionMap[MappedQuestionIds.OneThingCouldBeDoneToImproveSocialLife];

            var question = answerDTO.Question?.Title ?? MissingQuestionMessage;

            userAnswers.Add(answerDTO.QuestionId, new UserAnswerDetailViewModel
            {
                Question = question,
                Answer = answerPayloadDTO.Data,
                Flags = Enumerable.Empty<string>()
            });
        });

        // if (questionMap.ContainsKey(MappedQuestionIds.OneThingCouldBeDoneToImproveSocialLife))
        // {
        //     _logger.LogDebug("Applying rule {QuestionId}", MappedQuestionIds.OneThingCouldBeDoneToImproveSocialLife);
        //     
        //     answerDTO = questionMap[MappedQuestionIds.OneThingCouldBeDoneToImproveSocialLife];
        //     var answerPayloadDTO = Utility.DeserializeObject<AnswerPayloadDTO<string?>>(answerDTO.Data);
        //
        //     if (answerPayloadDTO is { Skipped: false } && !string.IsNullOrWhiteSpace(answerPayloadDTO.Data))
        //     {
        //         var question = answerDTO.Question?.Title ?? MissingQuestionMessage;
        //         
        //         userAnswers.Add(answerDTO.QuestionId, new UserAnswerDetailViewModel
        //         {
        //             Question = question,
        //             Answer = answerPayloadDTO.Data,
        //             Flags = Enumerable.Empty<string>()
        //         });
        //     }
        // }

        // Associated documentation link:
        // https://docs.google.com/spreadsheets/d/1Fzk5a6J58F1OlsCm7eA221gmc-v9T-08/edit#gid=1610914612&range=A106:Q106
        HandleAnswer<bool?>(questionMap, MappedQuestionIds.HaveYourPeersHurtYouQuestionId, answerPayloadDTO =>
        {
            if (answerPayloadDTO is null or { Skipped: true }) return;

            answerDTO = questionMap[MappedQuestionIds.HaveYourPeersHurtYouQuestionId];

            if (answerPayloadDTO is { Skipped: false, Data: false })
            {
                assessmentDataDTO.Pce++;
            }

            if (answerPayloadDTO is { Skipped: false, Data: true })
            {
                assessmentDataDTO.YouRisk++;
            }
        });

        // Associated documentation link:
        // https://docs.google.com/spreadsheets/d/1Fzk5a6J58F1OlsCm7eA221gmc-v9T-08/edit#gid=1610914612&range=A106:Q106
        // if (questionMap.ContainsKey(MappedQuestionIds.HaveYourPeersHurtYouQuestionId))
        // {
        //     _logger.LogDebug("Applying rule {QuestionId}", MappedQuestionIds.HaveYourPeersHurtYouQuestionId);
        //     
        //     answerDTO = questionMap[MappedQuestionIds.HaveYourPeersHurtYouQuestionId];
        //     var answerPayloadDTO = Utility.DeserializeObject<AnswerPayloadDTO<bool?>>(answerDTO.Data);
        //
        //     if (answerPayloadDTO is { Skipped: false, Data: false })
        //     {
        //         assessmentDataDTO.Pce++;
        //     }
        //
        //     if (answerPayloadDTO is { Skipped: false, Data: true })
        //     {
        //         assessmentDataDTO.YouRisk++;
        //     }
        // }

        // Associated documentation link:
        // https://docs.google.com/spreadsheets/d/1Fzk5a6J58F1OlsCm7eA221gmc-v9T-08/edit#gid=1610914612&range=A110:Q110
        HandleAnswer<int?>(questionMap, MappedQuestionIds.HowTiredOnTheWeekendQuestionId, answerPayloadDTO =>
        {
            if (answerPayloadDTO is null or { Skipped: true }) return;

            answerDTO = questionMap[MappedQuestionIds.HowTiredOnTheWeekendQuestionId];

            var scale = answerPayloadDTO.Data;
            if (scale is null) return;

            switch (scale)
            {
                case >= 4:
                    assessmentDataDTO.YouRisk++;
                    assessmentDataDTO.Depression++;
                    assessmentDataDTO.Internalizing++;

                    var question = answerDTO.Question?.Title ?? MissingQuestionMessage;
                    var answer = scale.ToString() ?? MissingAnswerMessage;

                    userAnswers.Add(answerDTO.QuestionId, new UserAnswerDetailViewModel
                    {
                        Question = question,
                        Answer = answer,
                        Flags = new[] { nameof(assessmentDataDTO.Depression) }
                    });

                    break;

                case <= 2:
                    assessmentDataDTO.YouProtective++;
                    break;
            }
        });

        // Associated documentation link:
        // https://docs.google.com/spreadsheets/d/1Fzk5a6J58F1OlsCm7eA221gmc-v9T-08/edit#gid=1610914612&range=A110:Q110
        // if (questionMap.ContainsKey(MappedQuestionIds.HowTiredOnTheWeekendQuestionId))
        // {
        //     _logger.LogDebug("Applying rule {QuestionId}", MappedQuestionIds.HowTiredOnTheWeekendQuestionId);
        //         
        //     answerDTO = questionMap[MappedQuestionIds.HowTiredOnTheWeekendQuestionId];
        //     var answerPayloadDTO = Utility.DeserializeObject<AnswerPayloadDTO<int?>>(answerDTO.Data);
        //     var scale = answerPayloadDTO.Data;
        //
        //     if (!answerPayloadDTO.Skipped && scale is not null)
        //     {
        //         switch (scale)
        //         {
        //             case >= 4:
        //                 assessmentDataDTO.YouRisk++;
        //                 assessmentDataDTO.Depression++;
        //                 assessmentDataDTO.Internalizing++;
        //                 
        //                 var question = answerDTO.Question?.Title ?? MissingQuestionMessage;
        //                 var answer = scale.ToString() ?? MissingAnswerMessage;
        //
        //                 userAnswers.Add(answerDTO.QuestionId, new UserAnswerDetailViewModel
        //                 {
        //                     Question = question,
        //                     Answer = answer,
        //                     Flags = new [] { nameof(assessmentDataDTO.Depression) }
        //                 });
        //
        //                 break;
        //
        //             case <= 2:
        //                 assessmentDataDTO.YouProtective++;
        //                 break;
        //         }
        //     }
        // }

        HandleAnswer<string?>(questionMap, MappedQuestionIds.WhatMakesYouTiredOnTheWeekendQuestionId,
            answerPayloadDTO =>
            {
                if (answerPayloadDTO is null or { Skipped: true } || string.IsNullOrWhiteSpace(answerPayloadDTO.Data))
                    return;

                answerDTO = questionMap[MappedQuestionIds.WhatMakesYouTiredOnTheWeekendQuestionId];

                var question = answerDTO.Question?.Title ?? MissingQuestionMessage;

                userAnswers.Add(answerDTO.QuestionId, new UserAnswerDetailViewModel
                {
                    Question = question,
                    Answer = answerPayloadDTO.Data,
                    Flags = Enumerable.Empty<string>()
                });
            });

        // if (questionMap.ContainsKey(MappedQuestionIds.WhatMakesYouTiredOnTheWeekendQuestionId))
        // {
        //     _logger.LogDebug("Applying rule {QuestionId}", MappedQuestionIds.WhatMakesYouTiredOnTheWeekendQuestionId);
        //     
        //     answerDTO = questionMap[MappedQuestionIds.WhatMakesYouTiredOnTheWeekendQuestionId];
        //     var answerPayloadDTO = Utility.DeserializeObject<AnswerPayloadDTO<string?>>(answerDTO.Data);
        //
        //     if (answerPayloadDTO is { Skipped: false } && !string.IsNullOrWhiteSpace(answerPayloadDTO.Data))
        //     {
        //         var question = answerDTO.Question?.Title ?? MissingQuestionMessage;
        //         
        //         userAnswers.Add(answerDTO.QuestionId, new UserAnswerDetailViewModel
        //         {
        //             Question = question,
        //             Answer = answerPayloadDTO.Data,
        //             Flags = Enumerable.Empty<string>()
        //         });
        //     }
        // }

        HandleAnswer<string?>(questionMap, MappedQuestionIds.WhatCouldYouChangeToBeLessTiredOnTheWeekendQuestionId,
            answerPayloadDTO =>
            {
                if (answerPayloadDTO is null or { Skipped: true } || string.IsNullOrWhiteSpace(answerPayloadDTO.Data))
                    return;

                answerDTO = questionMap[MappedQuestionIds.WhatCouldYouChangeToBeLessTiredOnTheWeekendQuestionId];

                var question = answerDTO.Question?.Title ?? MissingQuestionMessage;

                userAnswers.Add(answerDTO.QuestionId, new UserAnswerDetailViewModel
                {
                    Question = question,
                    Answer = answerPayloadDTO.Data,
                    Flags = Enumerable.Empty<string>()
                });
            });

        // if (questionMap.ContainsKey(MappedQuestionIds.WhatCouldYouChangeToBeLessTiredOnTheWeekendQuestionId))
        // {
        //     _logger.LogDebug("Applying rule {QuestionId}",
        //         MappedQuestionIds.WhatCouldYouChangeToBeLessTiredOnTheWeekendQuestionId);
        //     
        //     answerDTO = questionMap[MappedQuestionIds.WhatCouldYouChangeToBeLessTiredOnTheWeekendQuestionId];
        //     var answerPayloadDTO = Utility.DeserializeObject<AnswerPayloadDTO<string?>>(answerDTO.Data);
        //
        //     if (answerPayloadDTO is { Skipped: false } && !string.IsNullOrWhiteSpace(answerPayloadDTO.Data))
        //     {
        //         var question = answerDTO.Question?.Title ?? MissingQuestionMessage;
        //         
        //         userAnswers.Add(answerDTO.QuestionId, new UserAnswerDetailViewModel
        //         {
        //             Question = question,
        //             Answer = answerPayloadDTO.Data,
        //             Flags = Enumerable.Empty<string>()
        //         });
        //     }
        // }

        // Associated documentation link:
        // https://docs.google.com/spreadsheets/d/1Fzk5a6J58F1OlsCm7eA221gmc-v9T-08/edit#gid=1610914612&range=A113:Q113
        HandleAnswer<int?>(questionMap, MappedQuestionIds.HowSadAreYouQuestionId, answerPayloadDTO =>
        {
            if (answerPayloadDTO is null or { Skipped: true }) return;

            answerDTO = questionMap[MappedQuestionIds.HowSadAreYouQuestionId];

            var scale = answerPayloadDTO.Data;
            if (scale is null) return;

            switch (scale)
            {
                case >= 4:
                    assessmentDataDTO.YouRisk++;
                    assessmentDataDTO.Depression++;
                    assessmentDataDTO.Internalizing++;

                    var question = answerDTO.Question?.Title ?? MissingQuestionMessage;
                    var answer = scale.ToString() ?? MissingAnswerMessage;

                    userAnswers.Add(answerDTO.QuestionId, new UserAnswerDetailViewModel
                    {
                        Question = question,
                        Answer = answer,
                        Flags = new[]
                        {
                            nameof(assessmentDataDTO.Depression)
                        }
                    });

                    break;

                case <= 2:
                    assessmentDataDTO.YouProtective++;
                    break;
            }
        });

        // Associated documentation link:
        // https://docs.google.com/spreadsheets/d/1Fzk5a6J58F1OlsCm7eA221gmc-v9T-08/edit#gid=1610914612&range=A113:Q113
        // if (questionMap.ContainsKey(MappedQuestionIds.HowSadAreYouQuestionId))
        // {
        //     _logger.LogDebug("Applying rule {QuestionId}", MappedQuestionIds.HowSadAreYouQuestionId);
        //         
        //     answerDTO = questionMap[MappedQuestionIds.HowSadAreYouQuestionId];
        //     var answerPayloadDTO = Utility.DeserializeObject<AnswerPayloadDTO<int?>>(answerDTO.Data);
        //     var scale = answerPayloadDTO.Data;
        //
        //     if (!answerPayloadDTO.Skipped && scale is not null)
        //     {
        //         switch (scale)
        //         {
        //             case >= 4:
        //                 assessmentDataDTO.YouRisk++;
        //                 assessmentDataDTO.Depression++;
        //                 assessmentDataDTO.Internalizing++;
        //                 
        //                 var question = answerDTO.Question?.Title ?? MissingQuestionMessage;
        //                 var answer = scale.ToString() ?? MissingAnswerMessage;
        //
        //                 userAnswers.Add(answerDTO.QuestionId, new UserAnswerDetailViewModel
        //                 {
        //                     Question = question,
        //                     Answer = answer,
        //                     Flags = new []
        //                     {
        //                         nameof(assessmentDataDTO.Depression)
        //                     }
        //                 });
        //
        //                 break;
        //
        //             case <= 2:
        //                 assessmentDataDTO.YouProtective++;
        //                 break;
        //         }
        //     }
        // }

        HandleAnswer<string?>(questionMap, MappedQuestionIds.WhatMakesYouSadQuestionId, answerPayloadDTO =>
        {
            if (answerPayloadDTO is null or { Skipped: true } || string.IsNullOrWhiteSpace(answerPayloadDTO.Data))
                return;

            answerDTO = questionMap[MappedQuestionIds.WhatMakesYouSadQuestionId];

            var question = answerDTO.Question?.Title ?? MissingQuestionMessage;

            userAnswers.Add(answerDTO.QuestionId, new UserAnswerDetailViewModel
            {
                Question = question,
                Answer = answerPayloadDTO.Data,
                Flags = Enumerable.Empty<string>()
            });
        });

        // if (questionMap.ContainsKey(MappedQuestionIds.WhatMakesYouSadQuestionId))
        // {
        //     _logger.LogDebug("Applying rule {QuestionId}", MappedQuestionIds.WhatMakesYouSadQuestionId);
        //     
        //     answerDTO = questionMap[MappedQuestionIds.WhatMakesYouSadQuestionId];
        //     var answerPayloadDTO = Utility.DeserializeObject<AnswerPayloadDTO<string?>>(answerDTO.Data);
        //
        //     if (answerPayloadDTO is { Skipped: false } && !string.IsNullOrWhiteSpace(answerPayloadDTO.Data))
        //     {
        //         var question = answerDTO.Question?.Title ?? MissingQuestionMessage;
        //         
        //         userAnswers.Add(answerDTO.QuestionId, new UserAnswerDetailViewModel
        //         {
        //             Question = question,
        //             Answer = answerPayloadDTO.Data,
        //             Flags = Enumerable.Empty<string>()
        //         });
        //     }
        // }

        HandleAnswer<bool?>(questionMap, MappedQuestionIds.DoYouHaveInterestsQuestionId, answerPayloadDTO =>
        {
            switch (answerPayloadDTO)
            {
                case null or { Skipped: true }:
                    return;
                case { Skipped: false, Data: true }:
                    assessmentDataDTO.YouProtective++;
                    break;
            }
        });

        // if (questionMap.ContainsKey(MappedQuestionIds.DoYouHaveInterestsQuestionId))
        // {
        //     _logger.LogDebug("Applying rule {QuestionId}", MappedQuestionIds.DoYouHaveInterestsQuestionId);
        //     
        //     answerDTO = questionMap[MappedQuestionIds.DoYouHaveInterestsQuestionId];
        //     var answerPayloadDTO = Utility.DeserializeObject<AnswerPayloadDTO<bool?>>(answerDTO.Data);
        //
        //     if (answerPayloadDTO is { Skipped: false, Data: true })
        //     {
        //         assessmentDataDTO.YouProtective++;
        //     }
        // }

        HandleAnswer<string?>(questionMap, MappedQuestionIds.WhatAreTheyQuestionId, answerPayloadDTO =>
        {
            if (answerPayloadDTO is null or { Skipped: true } || string.IsNullOrWhiteSpace(answerPayloadDTO.Data))
                return;

            answerDTO = questionMap[MappedQuestionIds.WhatAreTheyQuestionId];

            var question = answerDTO.Question?.Title ?? MissingQuestionMessage;
            var answer = answerPayloadDTO.Data;

            userAnswers.Add(answerDTO.QuestionId, new UserAnswerDetailViewModel
            {
                Question = question,
                Answer = answer,
                Flags = Enumerable.Empty<string>()
            });
        });

        // if (questionMap.ContainsKey(MappedQuestionIds.WhatAreTheyQuestionId))
        // {
        //     _logger.LogDebug("Applying rule {QuestionId}", MappedQuestionIds.WhatAreTheyQuestionId);
        //     
        //     answerDTO = questionMap[MappedQuestionIds.WhatAreTheyQuestionId];
        //     var answerPayloadDTO = Utility.DeserializeObject<AnswerPayloadDTO<string?>>(answerDTO.Data);
        //
        //     if (answerPayloadDTO is { Skipped: false } && !string.IsNullOrWhiteSpace(answerPayloadDTO.Data))
        //     {
        //         var question = answerDTO.Question?.Title ?? MissingQuestionMessage;
        //         var answer = answerPayloadDTO.Data;
        //         
        //         userAnswers.Add(answerDTO.QuestionId, new UserAnswerDetailViewModel
        //         {
        //             Question = question,
        //             Answer = answer,
        //             Flags = Enumerable.Empty<string>()
        //         });
        //     }
        // }

        HandleAnswer<bool?>(questionMap, MappedQuestionIds.IsThereAnyThingThatStopsYouFromYourInterestsQuestionId,
            answerPayloadDTO =>
            {
                if (answerPayloadDTO is null or { Skipped: true } or { Data: false }) return;

                answerDTO = questionMap[MappedQuestionIds.IsThereAnyThingThatStopsYouFromYourInterestsQuestionId];

                assessmentDataDTO.YouRisk++;
                assessmentDataDTO.Anxiety++;
                assessmentDataDTO.Depression++;
                assessmentDataDTO.Internalizing++;
                assessmentDataDTO.Externalizing++;

                var question = answerDTO.Question?.Title ?? MissingQuestionMessage;
                var answer = answerPayloadDTO.Data == true ? "Yes" : "No";

                userAnswers.Add(answerDTO.QuestionId, new UserAnswerDetailViewModel
                {
                    Question = question,
                    Answer = answer,
                    Flags = Enumerable.Empty<string>()
                });
            });

        // if (questionMap.ContainsKey(MappedQuestionIds.IsThereAnyThingThatStopsYouFromYourInterestsQuestionId))
        // {
        //     _logger.LogDebug("Applying rule {QuestionId}",
        //         MappedQuestionIds.IsThereAnyThingThatStopsYouFromYourInterestsQuestionId);
        //     
        //     answerDTO = questionMap[MappedQuestionIds.IsThereAnyThingThatStopsYouFromYourInterestsQuestionId];
        //     var answerPayloadDTO = Utility.DeserializeObject<AnswerPayloadDTO<bool?>>(answerDTO.Data);
        //
        //     if (answerPayloadDTO is { Skipped: false, Data: true })
        //     {
        //         assessmentDataDTO.YouRisk++;
        //         assessmentDataDTO.Anxiety++;
        //         assessmentDataDTO.Depression++;
        //         assessmentDataDTO.Internalizing++;
        //         assessmentDataDTO.Externalizing++;
        //
        //         var question = answerDTO.Question?.Title ?? MissingQuestionMessage;
        //         var answer = answerPayloadDTO.Data == true ? "Yes" : "No";
        //         
        //         userAnswers.Add(answerDTO.QuestionId, new UserAnswerDetailViewModel
        //         {
        //             Question = question,
        //             Answer = answer,
        //             Flags = Enumerable.Empty<string>()
        //         });
        //     }
        // }

        // Associated documentation link:
        // https://docs.google.com/spreadsheets/d/1Fzk5a6J58F1OlsCm7eA221gmc-v9T-08/edit#gid=1610914612&range=A121:Q121
        HandleAnswer<int?>(questionMap, MappedQuestionIds.DoYouHaveAStomachAcheQuestionId, answerPayloadDTO =>
        {
            if (answerPayloadDTO is null or { Skipped: true }) return;

            answerDTO = questionMap[MappedQuestionIds.DoYouHaveAStomachAcheQuestionId];

            var scale = answerPayloadDTO.Data;
            if (scale is null) return;

            switch (scale)
            {
                case >= 4:
                    assessmentDataDTO.YouRisk++;
                    assessmentDataDTO.Anxiety++;
                    assessmentDataDTO.Internalizing++;

                    var question = answerDTO.Question?.Title ?? MissingQuestionMessage;
                    var answer = scale.ToString() ?? MissingAnswerMessage;

                    userAnswers.Add(answerDTO.QuestionId, new UserAnswerDetailViewModel
                    {
                        Question = question,
                        Answer = answer,
                        Flags = new[] { nameof(assessmentDataDTO.Anxiety) }
                    });

                    break;

                case <= 2:
                    assessmentDataDTO.YouProtective++;
                    break;
            }
        });

        // Associated documentation link:
        // https://docs.google.com/spreadsheets/d/1Fzk5a6J58F1OlsCm7eA221gmc-v9T-08/edit#gid=1610914612&range=A121:Q121
        // if (questionMap.ContainsKey(MappedQuestionIds.DoYouHaveAStomachAcheQuestionId))
        // {
        //     _logger.LogDebug("Applying rule {QuestionId}", MappedQuestionIds.DoYouHaveAStomachAcheQuestionId);
        //         
        //     answerDTO = questionMap[MappedQuestionIds.DoYouHaveAStomachAcheQuestionId];
        //     var answerPayloadDTO = Utility.DeserializeObject<AnswerPayloadDTO<int?>>(answerDTO.Data);
        //     var scale = answerPayloadDTO.Data;
        //
        //     if (!answerPayloadDTO.Skipped && scale is not null)
        //     {
        //         switch (scale)
        //         {
        //             case >= 4:
        //                 assessmentDataDTO.YouRisk++;
        //                 assessmentDataDTO.Anxiety++;
        //                 assessmentDataDTO.Internalizing++;
        //                 
        //                 var question = answerDTO.Question?.Title ?? MissingQuestionMessage;
        //                 var answer = scale.ToString() ?? MissingAnswerMessage;
        //                 
        //                 userAnswers.Add(answerDTO.QuestionId, new UserAnswerDetailViewModel
        //                 {
        //                     Question = question,
        //                     Answer = answer,
        //                     Flags = new [] {  nameof(assessmentDataDTO.Anxiety) }
        //                 });
        //
        //                 break;
        //
        //             case <= 2:
        //                 assessmentDataDTO.YouProtective++;
        //                 break;
        //         }
        //     }
        // }

        // Associated documentation link:
        // https://docs.google.com/spreadsheets/d/1Fzk5a6J58F1OlsCm7eA221gmc-v9T-08/edit#gid=1610914612&range=A122:Q122
        HandleAnswer<int?>(questionMap, MappedQuestionIds.FrequencyOfHeadachesQuestionId, answerPayloadDTO =>
        {
            if (answerPayloadDTO is null or { Skipped: true }) return;

            answerDTO = questionMap[MappedQuestionIds.FrequencyOfHeadachesQuestionId];

            var scale = answerPayloadDTO.Data;
            if (scale is null) return;

            switch (scale)
            {
                case >= 4:
                    assessmentDataDTO.YouRisk++;
                    assessmentDataDTO.Anxiety++;
                    assessmentDataDTO.Depression++;
                    assessmentDataDTO.Internalizing++;

                    var question = answerDTO.Question?.Title ?? MissingQuestionMessage;
                    var answer = scale.ToString() ?? MissingAnswerMessage;

                    userAnswers.Add(answerDTO.QuestionId, new UserAnswerDetailViewModel
                    {
                        Question = question,
                        Answer = answer,
                        Flags = new[]
                        {
                            nameof(assessmentDataDTO.Anxiety),
                            nameof(assessmentDataDTO.Depression)
                        }
                    });

                    break;

                case <= 2:
                    assessmentDataDTO.YouProtective++;
                    break;
            }
        });

        // Associated documentation link:
        // https://docs.google.com/spreadsheets/d/1Fzk5a6J58F1OlsCm7eA221gmc-v9T-08/edit#gid=1610914612&range=A122:Q122
        // if (questionMap.ContainsKey(MappedQuestionIds.FrequencyOfHeadachesQuestionId))
        // {
        //     _logger.LogDebug("Applying rule {QuestionId}", MappedQuestionIds.FrequencyOfHeadachesQuestionId);
        //         
        //     answerDTO = questionMap[MappedQuestionIds.FrequencyOfHeadachesQuestionId];
        //     var answerPayloadDTO = Utility.DeserializeObject<AnswerPayloadDTO<int?>>(answerDTO.Data);
        //     var scale = answerPayloadDTO.Data;
        //
        //     if (!answerPayloadDTO.Skipped && scale is not null)
        //     {
        //         switch (scale)
        //         {
        //             case >= 4:
        //                 assessmentDataDTO.YouRisk++;
        //                 assessmentDataDTO.Anxiety++;
        //                 assessmentDataDTO.Depression++;
        //                 assessmentDataDTO.Internalizing++;
        //                 
        //                 var question = answerDTO.Question?.Title ?? MissingQuestionMessage;
        //                 var answer = scale.ToString() ?? MissingAnswerMessage;
        //                 
        //                 userAnswers.Add(answerDTO.QuestionId, new UserAnswerDetailViewModel
        //                 {
        //                     Question = question,
        //                     Answer = answer,
        //                     Flags = new []
        //                     {
        //                         nameof(assessmentDataDTO.Anxiety),
        //                         nameof(assessmentDataDTO.Depression)
        //                     }
        //                 });
        //
        //                 break;
        //
        //             case <= 2:
        //                 assessmentDataDTO.YouProtective++;
        //                 break;
        //         }
        //     }
        // }

        // Associated documentation link:
        // https://docs.google.com/spreadsheets/d/1Fzk5a6J58F1OlsCm7eA221gmc-v9T-08/edit#gid=1610914612&range=A123:Q123
        HandleAnswer<IReadOnlyList<string>>(questionMap, MappedQuestionIds.CheckAllThatApplyQuestionId,
            answerPayloadDTO =>
            {
                if (answerPayloadDTO is null or { Skipped: true }) return;

                answerDTO = questionMap[MappedQuestionIds.CheckAllThatApplyQuestionId];

                var options = answerPayloadDTO.Data;

                if (options is null || !options.Any()) return;

                var question = answerDTO.Question?.Title ?? MissingQuestionMessage;
                var answers = new List<string>();
                var flags = new List<string>();

                if (options.Contains(CheckAllThatApply.CantControlThoughts))
                {
                    assessmentDataDTO.YouRisk++;
                    assessmentDataDTO.Anxiety++;
                    assessmentDataDTO.Depression++;
                    assessmentDataDTO.Adhd++;

                    const string answer = "I can't control my thoughts";
                    answers.Add(answer);

                    flags.Add(nameof(assessmentDataDTO.Adhd));
                    flags.Add(nameof(assessmentDataDTO.Anxiety));
                    flags.Add(nameof(assessmentDataDTO.Depression));
                }

                if (options.Contains(CheckAllThatApply.HeartRacing))
                {
                    assessmentDataDTO.YouRisk++;
                    assessmentDataDTO.Anxiety++;
                    assessmentDataDTO.Depression++;

                    const string answer = "My heart races uncomfortably";
                    answers.Add(answer);

                    flags.Add(nameof(assessmentDataDTO.Anxiety));
                    flags.Add(nameof(assessmentDataDTO.Depression));
                }

                if (options.Contains(CheckAllThatApply.SweatyPalms))
                {
                    assessmentDataDTO.YouRisk++;
                    assessmentDataDTO.Anxiety++;

                    const string answer = "My palms get sweaty when I worry";
                    answers.Add(answer);

                    flags.Add(nameof(assessmentDataDTO.Anxiety));
                }

                if (options.Contains(CheckAllThatApply.AlwaysTired))
                {
                    assessmentDataDTO.YouRisk++;
                    assessmentDataDTO.Depression++;

                    const string answer = "My palms get sweaty when I worry";
                    answers.Add(answer);

                    flags.Add(nameof(assessmentDataDTO.Depression));
                }

                var aggregatedAnswer = answers.Count > 0
                    ? $"{string.Join(", ", answers.Take(answers.Count - 1))}, and {answers[^1]}"
                    : answers[0];

                userAnswers.Add(answerDTO.QuestionId, new UserAnswerDetailViewModel
                {
                    Question = question,
                    Answer = aggregatedAnswer,
                    Flags = flags
                });
            });

        // Associated documentation link:
        // https://docs.google.com/spreadsheets/d/1Fzk5a6J58F1OlsCm7eA221gmc-v9T-08/edit#gid=1610914612&range=A123:Q123
        // if (questionMap.ContainsKey(MappedQuestionIds.CheckAllThatApplyQuestionId))
        // {
        //     _logger.LogDebug("Applying rule {QuestionId}", MappedQuestionIds.CheckAllThatApplyQuestionId);
        //     
        //     answerDTO = questionMap[MappedQuestionIds.CheckAllThatApplyQuestionId];
        //     var answerPayloadDTO = Utility.DeserializeObject<AnswerPayloadDTO<IReadOnlyList<string>>>(answerDTO.Data);
        //     var options = answerPayloadDTO.Data;
        //     if (!answerPayloadDTO.Skipped && options is not null && options.Any())
        //     {
        //         var question = answerDTO.Question?.Title ?? MissingQuestionMessage;
        //         var answers = new List<string>();
        //         var flags = new List<string>();
        //
        //         if (options.Contains(CheckAllThatApply.CantControlThoughts))
        //         {
        //             assessmentDataDTO.YouRisk++;
        //             assessmentDataDTO.Anxiety++;
        //             assessmentDataDTO.Depression++;
        //             assessmentDataDTO.Adhd++;
        //             
        //             const string answer = "I can't control my thoughts";
        //             answers.Add(answer);
        //             
        //             flags.Add(nameof(assessmentDataDTO.Adhd));
        //             flags.Add(nameof(assessmentDataDTO.Anxiety));
        //             flags.Add(nameof(assessmentDataDTO.Depression));
        //         }
        //
        //         if (options.Contains(CheckAllThatApply.HeartRacing))
        //         {
        //             assessmentDataDTO.YouRisk++;
        //             assessmentDataDTO.Anxiety++;
        //             assessmentDataDTO.Depression++;
        //             
        //             const string answer = "My heart races uncomfortably";
        //             answers.Add(answer);
        //             
        //             flags.Add(nameof(assessmentDataDTO.Anxiety));
        //             flags.Add(nameof(assessmentDataDTO.Depression));
        //         }
        //
        //         if (options.Contains(CheckAllThatApply.SweatyPalms))
        //         {
        //             assessmentDataDTO.YouRisk++;
        //             assessmentDataDTO.Anxiety++;
        //             
        //             const string answer = "My palms get sweaty when I worry";
        //             answers.Add(answer);  
        //             
        //             flags.Add(nameof(assessmentDataDTO.Anxiety));
        //         }
        //
        //         if (options.Contains(CheckAllThatApply.AlwaysTired))
        //         {
        //             assessmentDataDTO.YouRisk++;
        //             assessmentDataDTO.Depression++;
        //             
        //             const string answer = "My palms get sweaty when I worry";
        //             answers.Add(answer);
        //             
        //             flags.Add(nameof(assessmentDataDTO.Depression));
        //         }
        //
        //         var aggregatedAnswer = answers.Count > 0
        //             ? $"{string.Join(", ", answers.Take(answers.Count - 1))}, and {answers[^1]}"
        //             : answers[0];
        //         
        //         userAnswers.Add(answerDTO.QuestionId, new UserAnswerDetailViewModel
        //         {
        //             Question = question,
        //             Answer = aggregatedAnswer,
        //             Flags = flags
        //         });
        //     }
        // }

        // Associated documentation link:
        // https://docs.google.com/spreadsheets/d/1Fzk5a6J58F1OlsCm7eA221gmc-v9T-08/edit#gid=1610914612&range=A128:Q128
        HandleAnswer<bool?>(questionMap, MappedQuestionIds.HardToConcentrateQuestionId, answerPayloadDTO =>
        {
            if (answerPayloadDTO is null or { Skipped: true }) return;

            answerDTO = questionMap[MappedQuestionIds.HardToConcentrateQuestionId];

            var result = answerPayloadDTO.Data;
            if (result is null or false) return;

            assessmentDataDTO.YouRisk++;
            assessmentDataDTO.Anxiety++;
            assessmentDataDTO.Adhd++;
            assessmentDataDTO.Internalizing++;
            assessmentDataDTO.Externalizing++;

            var question = answerDTO.Question?.Title ?? MissingQuestionMessage;
            const string answer = "True";

            userAnswers.Add(answerDTO.QuestionId, new UserAnswerDetailViewModel
            {
                Question = question,
                Answer = answer,
                Flags = new[]
                {
                    nameof(assessmentDataDTO.Anxiety),
                    nameof(assessmentDataDTO.Adhd)
                }
            });
        });

        // Associated documentation link:
        // https://docs.google.com/spreadsheets/d/1Fzk5a6J58F1OlsCm7eA221gmc-v9T-08/edit#gid=1610914612&range=A128:Q128
        // if (questionMap.ContainsKey(MappedQuestionIds.HardToConcentrateQuestionId))
        // {
        //     _logger.LogDebug("Applying rule {QuestionId}", MappedQuestionIds.HardToConcentrateQuestionId);
        //         
        //     answerDTO = questionMap[MappedQuestionIds.HardToConcentrateQuestionId];
        //     var answerPayloadDTO = Utility.DeserializeObject<AnswerPayloadDTO<bool?>>(answerDTO.Data);
        //     var result = answerPayloadDTO.Data;
        //
        //     if (!answerPayloadDTO.Skipped && result is not null && result == true)
        //     {
        //         assessmentDataDTO.YouRisk++;
        //         assessmentDataDTO.Anxiety++;
        //         assessmentDataDTO.Adhd++;
        //         assessmentDataDTO.Internalizing++;
        //         assessmentDataDTO.Externalizing++;
        //         
        //         var question = answerDTO.Question?.Title ?? MissingQuestionMessage;
        //         var answer = "True";
        //                 
        //         userAnswers.Add(answerDTO.QuestionId, new UserAnswerDetailViewModel
        //         {
        //             Question = question,
        //             Answer = answer,
        //             Flags = new []
        //             {
        //                 nameof(assessmentDataDTO.Anxiety),
        //                 nameof(assessmentDataDTO.Adhd)
        //             }
        //         });
        //     }
        // }

        HandleAnswer<string?>(questionMap, MappedQuestionIds.HardToConcentrateDescriptionQuestionId, answerPayloadDTO =>
        {
            if (answerPayloadDTO is null or { Skipped: true } || string.IsNullOrWhiteSpace(answerPayloadDTO.Data))
                return;

            answerDTO = questionMap[MappedQuestionIds.HardToConcentrateDescriptionQuestionId];

            var question = answerDTO.Question?.Title ?? MissingQuestionMessage;

            userAnswers.Add(answerDTO.QuestionId, new UserAnswerDetailViewModel
            {
                Question = question,
                Answer = answerPayloadDTO.Data,
                Flags = Enumerable.Empty<string>()
            });
        });

        // if (questionMap.ContainsKey(MappedQuestionIds.HardToConcentrateDescriptionQuestionId))
        // {
        //     _logger.LogDebug("Applying rule {QuestionId}", MappedQuestionIds.HardToConcentrateDescriptionQuestionId);
        //         
        //     answerDTO = questionMap[MappedQuestionIds.HardToConcentrateDescriptionQuestionId];
        //     var answerPayloadDTO = Utility.DeserializeObject<AnswerPayloadDTO<string?>>(answerDTO.Data);
        //
        //     if (answerPayloadDTO is { Skipped: false } && !string.IsNullOrWhiteSpace(answerPayloadDTO.Data))
        //     {
        //         var question = answerDTO.Question?.Title ?? MissingQuestionMessage;
        //         
        //         userAnswers.Add(answerDTO.QuestionId, new UserAnswerDetailViewModel
        //         {
        //             Question = question,
        //             Answer = answerPayloadDTO.Data,
        //             Flags = Enumerable.Empty<string>()
        //         });
        //     }
        // }

        // Associated documentation link:
        // https://docs.google.com/spreadsheets/d/1Fzk5a6J58F1OlsCm7eA221gmc-v9T-08/edit#gid=1610914612&range=A132:Q132
        HandleAnswer<int?>(questionMap, MappedQuestionIds.HowNervousQuestionId, answerPayloadDTO =>
        {
            if (answerPayloadDTO is null or { Skipped: true }) return;

            answerDTO = questionMap[MappedQuestionIds.HowNervousQuestionId];

            var scale = answerPayloadDTO.Data;
            if (scale is null) return;

            switch (scale)
            {
                case >= 4:
                    assessmentDataDTO.YouRisk++;
                    assessmentDataDTO.Anxiety++;
                    assessmentDataDTO.Adhd++;

                    var question = answerDTO.Question?.Title ?? MissingQuestionMessage;
                    var answer = scale.ToString() ?? MissingAnswerMessage;

                    userAnswers.Add(answerDTO.QuestionId, new UserAnswerDetailViewModel
                    {
                        Question = question,
                        Answer = answer,
                        Flags = new[]
                        {
                            nameof(assessmentDataDTO.Anxiety),
                            nameof(assessmentDataDTO.Adhd)
                        }
                    });

                    break;

                case <= 2:
                    assessmentDataDTO.YouProtective++;
                    break;
            }
        });

        // Associated documentation link:
        // https://docs.google.com/spreadsheets/d/1Fzk5a6J58F1OlsCm7eA221gmc-v9T-08/edit#gid=1610914612&range=A132:Q132
        // if (questionMap.ContainsKey(MappedQuestionIds.HowNervousQuestionId))
        // {
        //     _logger.LogDebug("Applying rule {QuestionId}", MappedQuestionIds.HowNervousQuestionId);
        //                         
        //     answerDTO = questionMap[MappedQuestionIds.HowNervousQuestionId];
        //     var answerPayloadDTO = Utility.DeserializeObject<AnswerPayloadDTO<int?>>(answerDTO.Data);
        //     var scale = answerPayloadDTO.Data;
        //
        //     if (!answerPayloadDTO.Skipped && scale is not null)
        //     {
        //         switch (scale)
        //         {
        //             case >= 4:
        //                 assessmentDataDTO.YouRisk++;
        //                 assessmentDataDTO.Anxiety++;
        //                 assessmentDataDTO.Adhd++;
        //                 
        //                 var question = answerDTO.Question?.Title ?? MissingQuestionMessage;
        //                 var answer = scale.ToString() ?? MissingAnswerMessage;
        //                 
        //                 userAnswers.Add(answerDTO.QuestionId, new UserAnswerDetailViewModel
        //                 {
        //                     Question = question,
        //                     Answer = answer,
        //                     Flags = new []
        //                     {
        //                         nameof(assessmentDataDTO.Anxiety),
        //                         nameof(assessmentDataDTO.Adhd)
        //                     }
        //                 });
        //                 
        //                 break;
        //
        //             case <= 2:
        //                 assessmentDataDTO.YouProtective++;
        //                 break;
        //         }
        //     }
        // }

        HandleAnswer<string?>(questionMap, MappedQuestionIds.HowNervousDescriptionQuestionId, answerPayloadDTO =>
        {
            if (answerPayloadDTO is null or { Skipped: true } || string.IsNullOrWhiteSpace(answerPayloadDTO.Data))
                return;

            answerDTO = questionMap[MappedQuestionIds.HowNervousDescriptionQuestionId];

            var question = answerDTO.Question?.Title ?? MissingQuestionMessage;

            userAnswers.Add(answerDTO.QuestionId, new UserAnswerDetailViewModel
            {
                Question = question,
                Answer = answerPayloadDTO.Data,
                Flags = Enumerable.Empty<string>()
            });
        });

        // if (questionMap.ContainsKey(MappedQuestionIds.HowNervousDescriptionQuestionId))
        // {
        //     _logger.LogDebug("Applying rule {QuestionId}", MappedQuestionIds.HowNervousDescriptionQuestionId);
        //         
        //     answerDTO = questionMap[MappedQuestionIds.HowNervousDescriptionQuestionId];
        //     var answerPayloadDTO = Utility.DeserializeObject<AnswerPayloadDTO<string?>>(answerDTO.Data);
        //
        //     if (answerPayloadDTO is { Skipped: false } && !string.IsNullOrWhiteSpace(answerPayloadDTO.Data))
        //     {
        //         var question = answerDTO.Question?.Title ?? MissingQuestionMessage;
        //         
        //         userAnswers.Add(answerDTO.QuestionId, new UserAnswerDetailViewModel
        //         {
        //             Question = question,
        //             Answer = answerPayloadDTO.Data,
        //             Flags = Enumerable.Empty<string>()
        //         });
        //     }
        // }

        HandleAnswer<string?>(questionMap, MappedQuestionIds.ChangeToBeLessNervousQuestionId, answerPayloadDTO =>
        {
            if (answerPayloadDTO is null or { Skipped: true } || string.IsNullOrWhiteSpace(answerPayloadDTO.Data))
                return;

            answerDTO = questionMap[MappedQuestionIds.ChangeToBeLessNervousQuestionId];

            var question = answerDTO.Question?.Title ?? MissingQuestionMessage;

            userAnswers.Add(answerDTO.QuestionId, new UserAnswerDetailViewModel
            {
                Question = question,
                Answer = answerPayloadDTO.Data,
                Flags = Enumerable.Empty<string>()
            });
        });

        // if (questionMap.ContainsKey(MappedQuestionIds.ChangeToBeLessNervousQuestionId))
        // {
        //     _logger.LogDebug("Applying rule {QuestionId}", MappedQuestionIds.ChangeToBeLessNervousQuestionId);
        //         
        //     answerDTO = questionMap[MappedQuestionIds.ChangeToBeLessNervousQuestionId];
        //     var answerPayloadDTO = Utility.DeserializeObject<AnswerPayloadDTO<string?>>(answerDTO.Data);
        //
        //     if (answerPayloadDTO is { Skipped: false } && !string.IsNullOrWhiteSpace(answerPayloadDTO.Data))
        //     {
        //         var question = answerDTO.Question?.Title ?? MissingQuestionMessage;
        //         
        //         userAnswers.Add(answerDTO.QuestionId, new UserAnswerDetailViewModel
        //         {
        //             Question = question,
        //             Answer = answerPayloadDTO.Data,
        //             Flags = Enumerable.Empty<string>()
        //         });
        //     }
        // }

        HandleAnswer<string?>(questionMap, MappedQuestionIds.GreatestWorryQuestionId, answerPayloadDTO =>
        {
            if (answerPayloadDTO is null or { Skipped: true } || string.IsNullOrWhiteSpace(answerPayloadDTO.Data))
                return;

            answerDTO = questionMap[MappedQuestionIds.GreatestWorryQuestionId];

            var question = answerDTO.Question?.Title ?? MissingQuestionMessage;

            userAnswers.Add(answerDTO.QuestionId, new UserAnswerDetailViewModel
            {
                Question = question,
                Answer = answerPayloadDTO.Data,
                Flags = Enumerable.Empty<string>()
            });
        });

        // if (questionMap.ContainsKey(MappedQuestionIds.GreatestWorryQuestionId))
        // {
        //     _logger.LogDebug("Applying rule {QuestionId}", MappedQuestionIds.GreatestWorryQuestionId);
        //     
        //     answerDTO = questionMap[MappedQuestionIds.GreatestWorryQuestionId];
        //     var answerPayloadDTO = Utility.DeserializeObject<AnswerPayloadDTO<string?>>(answerDTO.Data);
        //
        //     if (answerPayloadDTO is { Skipped: false } && !string.IsNullOrWhiteSpace(answerPayloadDTO.Data))
        //     {
        //         var question = answerDTO.Question?.Title ?? MissingQuestionMessage;
        //         
        //         userAnswers.Add(answerDTO.QuestionId, new UserAnswerDetailViewModel
        //         {
        //             Question = question,
        //             Answer = answerPayloadDTO.Data,
        //             Flags = Enumerable.Empty<string>()
        //         });
        //     }
        // }

        // Associated documentation link:
        // https://docs.google.com/spreadsheets/d/1Fzk5a6J58F1OlsCm7eA221gmc-v9T-08/edit#gid=1610914612&range=A136:Q136
        HandleAnswer<bool?>(questionMap, MappedQuestionIds.WorryStoppedSocializationQuestionId, answerPayloadDTO =>
        {
            if (answerPayloadDTO is null or { Skipped: true }) return;

            answerDTO = questionMap[MappedQuestionIds.WorryStoppedSocializationQuestionId];

            var result = answerPayloadDTO.Data;
            if (result is null or false) return;

            assessmentDataDTO.YouRisk++;
            assessmentDataDTO.Anxiety++;
            assessmentDataDTO.Depression++;
            assessmentDataDTO.Internalizing++;
            assessmentDataDTO.Externalizing++;

            var question = answerDTO.Question?.Title ?? MissingQuestionMessage;
            const string answer = "True";

            userAnswers.Add(answerDTO.QuestionId, new UserAnswerDetailViewModel
            {
                Question = question,
                Answer = answer,
                Flags = new[]
                {
                    nameof(assessmentDataDTO.Anxiety),
                    nameof(assessmentDataDTO.Depression)
                }
            });
        });

        // Associated documentation link:
        // https://docs.google.com/spreadsheets/d/1Fzk5a6J58F1OlsCm7eA221gmc-v9T-08/edit#gid=1610914612&range=A136:Q136
        // if (questionMap.ContainsKey(MappedQuestionIds.WorryStoppedSocializationQuestionId))
        // {
        //     _logger.LogDebug("Applying rule {QuestionId}", MappedQuestionIds.WorryStoppedSocializationQuestionId);
        //         
        //     answerDTO = questionMap[MappedQuestionIds.WorryStoppedSocializationQuestionId];
        //     var answerPayloadDTO = Utility.DeserializeObject<AnswerPayloadDTO<bool?>>(answerDTO.Data);
        //     var result = answerPayloadDTO.Data;
        //
        //     if (!answerPayloadDTO.Skipped && result is not null && result == true)
        //     {
        //         assessmentDataDTO.YouRisk++;
        //         assessmentDataDTO.Anxiety++;
        //         assessmentDataDTO.Depression++;
        //         assessmentDataDTO.Internalizing++;
        //         assessmentDataDTO.Externalizing++;
        //         
        //         var question = answerDTO.Question?.Title ?? MissingQuestionMessage;
        //         const string answer = "True";
        //         
        //         userAnswers.Add(answerDTO.QuestionId, new UserAnswerDetailViewModel
        //         {
        //             Question = question,
        //             Answer = answer,
        //             Flags = new []
        //             {
        //                 nameof(assessmentDataDTO.Anxiety),
        //                 nameof(assessmentDataDTO.Depression)
        //             }
        //         });
        //     }
        // }

        // Associated documentation link:
        // https://docs.google.com/spreadsheets/d/1Fzk5a6J58F1OlsCm7eA221gmc-v9T-08/edit#gid=1610914612&range=A139:Q139
        HandleAnswer<bool?>(questionMap, MappedQuestionIds.SuicidalQuestionId, answerPayloadDTO =>
        {
            if (answerPayloadDTO is null or { Skipped: true }) return;

            answerDTO = questionMap[MappedQuestionIds.SuicidalQuestionId];

            if (answerPayloadDTO is not { Skipped: false, Data: true }) return;

            assessmentDataDTO.YouRisk++;
            assessmentDataDTO.Depression++;
            assessmentDataDTO.Internalizing++;
            assessmentDataDTO.Suicidality++;

            var question = answerDTO.Question?.Title ?? MissingQuestionMessage;
            const string answer = "True";

            userAnswers.Add(answerDTO.QuestionId, new UserAnswerDetailViewModel
            {
                Question = question,
                Answer = answer,
                Flags = new[]
                {
                    nameof(assessmentDataDTO.Depression),
                    nameof(assessmentDataDTO.Suicidality)
                }
            });
        });

        // Associated documentation link:
        // https://docs.google.com/spreadsheets/d/1Fzk5a6J58F1OlsCm7eA221gmc-v9T-08/edit#gid=1610914612&range=A139:Q139
        // if (questionMap.ContainsKey(MappedQuestionIds.SuicidalQuestionId))
        // {
        //     _logger.LogDebug("Applying rule {QuestionId}", MappedQuestionIds.SuicidalQuestionId);
        //         
        //     answerDTO = questionMap[MappedQuestionIds.SuicidalQuestionId];
        //     var answerPayloadDTO = Utility.DeserializeObject<AnswerPayloadDTO<bool?>>(answerDTO.Data);
        //     var result = answerPayloadDTO.Data;
        //
        //     if (answerPayloadDTO is { Skipped: false, Data: true })
        //     {
        //         assessmentDataDTO.YouRisk++;
        //         assessmentDataDTO.Depression++;
        //         assessmentDataDTO.Internalizing++;
        //         assessmentDataDTO.Suicidality++;
        //         
        //         var question = answerDTO.Question?.Title ?? MissingQuestionMessage;
        //         const string answer = "True";
        //         
        //         userAnswers.Add(answerDTO.QuestionId, new UserAnswerDetailViewModel
        //         {
        //             Question = question,
        //             Answer = answer,
        //             Flags = new []
        //             {
        //                 nameof(assessmentDataDTO.Depression),
        //                 nameof(assessmentDataDTO.Suicidality)
        //             }
        //         });
        //     }
        // }

        // Associated documentation link:
        // https://docs.google.com/spreadsheets/d/1Fzk5a6J58F1OlsCm7eA221gmc-v9T-08/edit#gid=1610914612&range=A142:Q142
        HandleAnswer<string?>(questionMap, MappedQuestionIds.SuicidalDescriptionQuestionId, answerPayloadDTO =>
        {
            if (answerPayloadDTO is null or { Skipped: true } || string.IsNullOrWhiteSpace(answerPayloadDTO.Data))
                return;

            answerDTO = questionMap[MappedQuestionIds.SuicidalDescriptionQuestionId];

            var question = answerDTO.Question?.Title ?? MissingQuestionMessage;
            var answer = answerPayloadDTO.Data;

            userAnswers.Add(answerDTO.QuestionId, new UserAnswerDetailViewModel
            {
                Question = question,
                Answer = answer,
                Flags = Enumerable.Empty<string>()
            });
        });

        // Associated documentation link:
        // https://docs.google.com/spreadsheets/d/1Fzk5a6J58F1OlsCm7eA221gmc-v9T-08/edit#gid=1610914612&range=A142:Q142
        // if (questionMap.ContainsKey(MappedQuestionIds.SuicidalDescriptionQuestionId))
        // {
        //     _logger.LogDebug("Applying rule {QuestionId}", MappedQuestionIds.SuicidalDescriptionQuestionId);
        //         
        //     answerDTO = questionMap[MappedQuestionIds.SuicidalDescriptionQuestionId];
        //     var answerPayloadDTO = Utility.DeserializeObject<AnswerPayloadDTO<string>>(answerDTO.Data);
        //
        //     if (!answerPayloadDTO.Skipped && !string.IsNullOrWhiteSpace(answerPayloadDTO.Data))
        //     {
        //         var question = answerDTO.Question?.Title ?? MissingQuestionMessage;
        //         var answer = answerPayloadDTO.Data;
        //
        //         userAnswers.Add(answerDTO.QuestionId, new UserAnswerDetailViewModel
        //         {
        //             Question = question,
        //             Answer = answer,
        //             Flags = Enumerable.Empty<string>()
        //         });
        //     }
        // }

        return Tuple.Create(assessmentDataDTO, userAnswers);
    }
}

/// <summary>
/// Model used to replace template tokens in the TWS Session Report
/// See https://www.figma.com/file/5JMiXjTY1Zofa7ErpkQWH7/Client-Report?node-id=3%3A152
/// </summary>
public class ReportViewModel
{
    /// <summary>
    /// Represents the Students name (also used with "for")
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Assessment Date
    /// </summary>
    public DateTime AssessmentDate { get; set; }

    /// <summary>
    /// Students grade
    /// </summary>
    public string Grade { get; set; } = null!;

    /// <summary>
    /// Students date of birth
    /// </summary>
    public DateTime DateOfBirth { get; set; }

    /// <summary>
    /// Who the student lives with
    /// </summary>
    public string? LivesWith { get; set; }

    /// <summary>
    /// Total number of siblings the student has
    /// </summary>
    public int TotalSiblings { get; set; }

    /// <summary>
    /// If there was a death in the family
    /// </summary>
    public bool HasDeathInFamily { get; set; }

    /// <summary>
    /// Related to Death in the family
    /// </summary>
    public string? Details { get; set; }

    public string? DeathInTheFamilyWho { get; set; }

    public string? DeathInTheFamilyWhen { get; set; }

    public int FeProtectiveTotal { get; set; }
    public int FeFeRiskTotal { get; set; }

    public int YouProtectiveTotal { get; set; }
    public int YouRiskTotal { get; set; }

    /// <summary>
    /// Used in the protective risk section of the report
    /// </summary>
    public int EnvironmentalRiskRatio { get; set; }

    /// <summary>
    /// Used in the protective risk section of the report
    /// </summary>
    public int PersonalRiskRatio { get; set; }

    /// <summary>
    /// If the child is seeing a mental health professional
    /// </summary>
    public bool IsSeeingMentalHealthProfessional { get; set; }

    /// <summary>
    /// Used for the "Environmental Risk Factor" section of the report
    /// </summary>
    public IEnumerable<string> EnvironmentalRiskFactorFlags { get; set; } = null!;

    /// <summary>
    /// Used for the "Social Risk Factors" section of the report
    /// </summary>
    public List<FactorFlagsViewModel> SocialRiskFactorFlags { get; set; } = new();

    /// <summary>
    /// Used for the "Adverse Childhood Experiences" section of the report
    /// </summary>
    public IEnumerable<string> Ace { get; set; }

    /// <summary>
    /// Used for the "Positive Childhood Experiences" section of the report
    /// </summary>
    public IEnumerable<string> Pce { get; set; }

    /// <summary>
    /// Used to hold all user's answers.
    /// </summary>
    public List<UserAnswerDetailViewModel> UserAnswers { get; set; } = new();

    /// <summary>
    /// Used for the "Reported strengths, gifts and talents:" section of the report
    /// </summary>
    public string? ReportedGiftsStrengthsTalents { get; set; }

    /// <summary>
    /// Used for the "What gives you strength?" question under the positive childhood experiences section
    /// </summary>
    public string? ReportedWhatGivesStrength { get; set; }

    /// <summary>
    /// Used for the "What gives you energy?" question under the positive childhood experiences section
    /// </summary>
    public string? ReportedWhatGivesEnergy { get; set; }

    /// <summary>
    /// Used for the "Risks Assessed*" section of the report, the "Anxiety" property
    /// </summary>
    public int AnxietyScale { get; set; }

    /// <summary>
    /// Used for the "Risks Assessed*" section of the report, the "Depression" property
    /// </summary>
    public int DepressionScale { get; set; }

    /// <summary>
    /// Holds flags associated with Risk Assessment section of the report
    /// </summary>
    public List<string> OtherRisksFlagged { get; set; } = new();

    public List<ReportTag> ReportTags { get; set; } = new();

    /// <summary>
    /// Used for the "Positive Childhood Experiences," people out side of the age of 18 section of the report
    /// </summary>
    public IEnumerable<string> PeopleOverTheAgeOf18 { get; set; }

    public Dictionary<string, List<RiskResponseDetailViewModel>> RiskQuestionResponses { get; set; }
}

public class RiskResponseDetailViewModel
{
    public string Question { get; set; }
    public string Answer { get; set; }
}

public class UserAnswerDetailViewModel
{
    public string Question { get; set; } = null!;
    public string Answer { get; set; } = null!;
    public IEnumerable<string> Flags { get; set; } = null!;
}

public class FactorFlagsViewModel
{
    public string Label { get; set; } = null!;
    public string? Details { get; set; }
}

public class ReportTag
{
    public string Type { get; set; } = null!;
    public string Name { get; set; } = null!;
}