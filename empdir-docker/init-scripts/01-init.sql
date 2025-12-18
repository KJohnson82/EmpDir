-- EmpDir PostgreSQL Initialization Script
-- This script runs automatically when the container is first created

-- Enable useful extensions
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- Grant privileges to the application user
GRANT ALL PRIVILEGES ON DATABASE empdir_db TO empdir_user;

-- Log that initialization completed
DO $$
BEGIN
    RAISE NOTICE '================================================';
    RAISE NOTICE 'EmpDir PostgreSQL database initialized!';
    RAISE NOTICE 'Database: empdir_db';
    RAISE NOTICE 'User: empdir_user';
    RAISE NOTICE '================================================';
END $$;
