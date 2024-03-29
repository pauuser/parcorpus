﻿using Parcorpus.API.Dto;
using Parcorpus.Core.Models;

namespace Parcorpus.API.Converters;

public static class FilterConverter
{
    public static Filter ConvertDtoToAppModel(FilterDto? filter)
    {
        return new Filter(filter?.Genre, filter?.StartYear, filter?.EndYear, filter?.Author);
    }
    
    public static FilterDto ConvertAppModelToDto(Filter? filter)
    {
        return new FilterDto(filter?.Genre, filter?.StartYear, filter?.EndYear, filter?.Author);
    }
}