﻿using Parcorpus.Core.Models;

namespace Parcorpus.Core.Interfaces;

public interface IAnnotationService
{
    Task<List<Sentence>> AlignSentences(BiText biText);
}