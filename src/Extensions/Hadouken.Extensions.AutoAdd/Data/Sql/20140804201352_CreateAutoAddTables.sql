/*
Creates the initial table structure for AutoAdd
folders and history.
*/

CREATE TABLE AutoAdd_Folder (
    Id                  INTEGER     PRIMARY KEY,
    [Path]              TEXT        NOT NULL,
    Pattern             TEXT        NULL,
    RemoveSourceFile    BOOLEAN     NOT NULL,
	RecursiveSearch     BOOLEAN     NOT NULL,
    AutoStart           BOOLEAN     NOT NULL
);

CREATE TABLE AutoAdd_History (
    Id                  INTEGER     PRIMARY KEY,
	[Path]              TEXT        NOT NULL,
	AddedTime           DATETIME    NOT NULL
);