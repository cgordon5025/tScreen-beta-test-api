using System;

namespace GraphQl.GraphQl.Features.Objects.WorkList.Inputs;

public record EditWorkListNoteInput(Guid Id, Guid WorkListId, NoteType Type, string Body, string? Data);