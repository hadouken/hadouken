/*
Creates the initial table structure for RSS feeds
and filters.
*/

CREATE TABLE Rss_Feed (
    Id              INTEGER     PRIMARY KEY,
    Name            TEXT        NOT NULL,
    Url             TEXT        NOT NULL,
    PollInterval    INTEGER     NOT NULL,
    LastUpdatedTime DATETIME    NOT NULL
);

CREATE TABLE Rss_Filter (
    Id              INTEGER     PRIMARY KEY,
    FeedId          INTEGER     NOT NULL,
    IncludePattern  TEXT        NULL,
    ExcludePattern  TEXT        NULL,
    AutoStart       BOOLEAN     NOT NULL,
    FOREIGN KEY (FeedId) REFERENCES Rss_Feed(Id)
);

CREATE TABLE Rss_Modifier (
    Id              INTEGER     PRIMARY KEY,
    FilterId        INTEGER     NOT NULL,
    [Target]        INTEGER     NOT NULL,
    Value           TEXT        NOT NULL,
    FOREIGN KEY (FilterId) REFERENCES Rss_Filter(Id)
);