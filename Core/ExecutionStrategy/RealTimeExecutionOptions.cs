﻿namespace Core.ExecutionStrategy;

public class RealTimeExecutionOptions : ExecutionOptions
{
    public TimeSpan RequestCycleTime { get; set; }

    public TimeSpan PresentationCycleTime { get; set; }
}