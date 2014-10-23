CREATE TABLE Notifier (
    NotifierId TEXT NOT NULL,
    NotificationType TEXT NOT NULL,
    UNIQUE (NotifierId, NotificationType) ON CONFLICT REPLACE
);