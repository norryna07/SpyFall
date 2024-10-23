DO $$
DECLARE 
    counter INT := 1;  -- Declare and initialize the counter
BEGIN
    WHILE counter <= 65532 LOOP
        INSERT INTO CommonServices (PortNumber)
        VALUES (counter);
        
        counter := counter + 1;  -- Increment the counter
    END LOOP;
END $$;
