CREATE TABLE ServiceVerif (
    servID SERIAL PRIMARY KEY,
    Name VARCHAR(20) NOT NULL,
    sendMessage BYTEA NULL,
    containResponse BYTEA NULL,
    startResponse BYTEA NULL,
    minLengthResponse INT NOT NULL,
    SendSize INT NULL
);

CREATE TABLE CommonServices (
    PortNumber NUMERIC(18, 0) NOT NULL PRIMARY KEY,
    ServiceName VARCHAR(50) DEFAULT 'unknown' NOT NULL
);
