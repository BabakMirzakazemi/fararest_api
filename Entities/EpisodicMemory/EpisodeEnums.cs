namespace Entities.EpisodicMemory;

public enum EpisodeType
{
    ArchitecturalDecision = 1,
    BugDiscovered = 2,
    BugFixed = 3,
    Migration = 4,
    Incident = 5,
    FailedAttempt = 6,
    SuccessfulImplementation = 7,
    DependencyUpgrade = 8,
    AgentAction = 9,
    DeploymentEvent = 10,
    OperationalLearning = 11,
    PerformanceFinding = 12,
    SecurityFinding = 13
}

public enum EpisodeImportance
{
    Low = 1,
    Medium = 2,
    High = 3,
    Critical = 4
}

public enum EpisodeSource
{
    Human = 1,
    Agent = 2,
    System = 3,
    DeploymentPipeline = 4
}

public enum EpisodeStatus
{
    Open = 1,
    Resolved = 2,
    Superseded = 3,
    Informational = 4
}

public enum EpisodeReferenceType
{
    Module = 1,
    Entity = 2,
    Table = 3,
    Migration = 4,
    ApiEndpoint = 5,
    Controller = 6,
    Service = 7,
    File = 8,
    GraphNodeHint = 9,
    Ticket = 10,
    Incident = 11,
    Deployment = 12,
    Commit = 13
}
