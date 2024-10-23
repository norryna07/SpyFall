CREATE TABLE ServiceVerif (
    servID INT NOT NULL PRIMARY KEY,
    Name NCHAR(20) NOT NULL,
    sendMessage BINARY(100) NULL,
    containResponse BINARY(50) NULL,
    startResponse BINARY(50) NULL,
    minLengthResponse INT NOT NULL,
    SendSize INT NULL
);

CREATE TABLE CommonServices (
    PortNumber NUMERIC(18, 0) NOT NULL PRIMARY KEY,
    ServiceName NCHAR(50) NOT NULL
);