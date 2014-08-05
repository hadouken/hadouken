/*
Creates the Setting table which is used by the KeyValueStore.
*/

CREATE TABLE Setting (
	[Key]    TEXT    NOT NULL,
	Value    TEXT    NOT NULL,
	UNIQUE([Key])
);