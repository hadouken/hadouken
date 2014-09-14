CREATE TABLE AutoMove_Rule (
    Id INTEGER PRIMARY KEY,
    Name TEXT NOT NULL,
    TargetPath TEXT NOT NULL
);

CREATE TABLE AutoMove_Parameter (
    Id INTEGER PRIMARY KEY,
    RuleId INTEGER NOT NULL,
    Source INTEGER NOT NULL,
    Pattern TEXT NOT NULL
);