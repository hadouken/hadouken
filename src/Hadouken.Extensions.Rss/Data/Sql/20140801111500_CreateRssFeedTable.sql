/*
Creates the initial table structure for RSS feeds
and filters.
*/

CREATE TABLE Rss_Feed (
    Id              INTEGER   PRIMARY KEY,
    Name            TEXT      NOT NULL,
    Uri             TEXT      NOT NULL,
    PollInterval    TEXT      NOT NULL
);