# Historical source — reference only

Build and run the active `RentOnCars/RentOnCars.sln` at the repository root. This directory is excluded from delivery and CI.

`dbo.database.sql.disabled` is a destructive historic script that drops customer tables. Its extension intentionally prevents it being mistaken for the supported setup script. Do not execute it. The supported, non-destructive schema is `RentOnCars/dbo.database.sql`.
