INSERT INTO ServiceVerif (Name, sendMessage, containResponse, startResponse, minLengthResponse, sendSize) VALUES 
('FTP', '\x48454c4c4f0d0a', NULL, '\x32', 0, 7),
('Telnet', '\xfffd18', '\xfffb18', NULL, 0, 3),
('Telnet', '\xfffd18', '\xfffc18', NULL, 0, 3),
('Telnet', '\xfffb18', '\xfffd18', NULL, 0, 3),
('Telnet', '\xfffb18', '\xfffe18', NULL, 0, 3),
('SSH', NULL, '\x5353482D322E302D', NULL, 0, 8),
('NTP', '\x1B', NULL, NULL, 48, 1),
('SMTP', '\x48454C4F206578616D706C652E636F6D', NULL, '\x32', 3, 16),
('SMTP', '\x45484C4F206578616D706C652E636F6D', NULL, '\x32', 3, 16),
('HTTP', '\x474554202F20485454502F312E310D0A0D0A', '\x485454502F', NULL, 1, 18),
('HTTPS', '\x474554202F20485454502F312E310D0A0D0A', NULL, NULL, 1, 18),
('DNS', '\xDB420100000100000000000006676F6F676C6503636F6D0000010001', NULL, NULL, 4, 28),
('SQL Microsoft', '\x12010034000001', NULL, '\x12', 1, 7),
('PostgresSQL', '\x00030000706F73746772657300706F73746772657300706F7374677265730000', NULL, NULL, 1, 32);