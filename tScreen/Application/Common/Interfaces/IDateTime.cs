﻿using System;

namespace Application.Common.Interfaces
{
    public interface IDateTime
    {
        public DateTime Now();
        public DateTime NowUtc();
    }
}