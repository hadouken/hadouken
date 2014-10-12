/*
Creates the User table.
*/

CREATE TABLE [User] (
    Id              TEXT        PRIMARY KEY,
    UserName        TEXT        NOT NULL,
    HashedPassword  TEXT        NOT NULL
);