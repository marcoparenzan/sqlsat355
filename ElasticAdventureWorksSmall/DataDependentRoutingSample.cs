/*
    Copyright 2014 Microsoft, Corp.

    Licensed under the Apache License, Version 2.0 (the "License");
    you may not use this file except in compliance with the License.
    You may obtain a copy of the License at

        http://www.apache.org/licenses/LICENSE-2.0

    Unless required by applicable law or agreed to in writing, software
    distributed under the License is distributed on an "AS IS" BASIS,
    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
    See the License for the specific language governing permissions and
    limitations under the License.
*/

using System;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.Azure.SqlDatabase.ElasticScale.ShardManagement;

namespace ElasticAdventureWorksSmall
{
    internal static class DataDependentRoutingSample
    {
        private static Random r = new Random();
        public static void ExecuteDataDependentRoutingQuery(RangeShardMap<int> shardMap, string credentialsConnectionString)
        {
            var dc = new sqlsat355Entities();
            var employees = dc.vEmployee.ToList();
            
            int currentMaxHighKey = shardMap.GetMappings().Max(m => m.Value.High);
            int employeeId = employees[r.Next(employees.Count)].BusinessEntityID;
            string employeeName = employees[r.Next(employees.Count)].LastName + " " + employees[r.Next(employees.Count)].FirstName;
            int regionId = 0;

            AddEmployee(
                shardMap,
                credentialsConnectionString,
                employeeId,
                employeeName,
                regionId);
        }

        /// <summary>
        /// Adds a customer to the customers table (or updates the customer if that id already exists).
        /// </summary>
        private static void AddEmployee(
            ShardMap shardMap,
            string credentialsConnectionString,
            int employeeId,
            string name,
            int regionId)
        {
            // Open and execute the command with retry for transient faults. Note that if the command fails, the connection is closed, so
            // the entire block is wrapped in a retry. This means that only one command should be executed per block, since if we had multiple
            // commands then the first command may be executed multiple times if later commands fail.
            SqlDatabaseUtils.SqlRetryPolicy.ExecuteAction(() =>
            {
                // Looks up the key in the shard map and opens a connection to the shard
                using (SqlConnection conn = shardMap.OpenConnectionForKey(employeeId, credentialsConnectionString))
                {
                    // Create a simple command that will insert or update the customer information
                    SqlCommand cmd = conn.CreateCommand();
                    cmd.CommandText = @"
                    IF EXISTS (SELECT 1 FROM [HumanResources].[vEmployee] WHERE [BusinessEntityID] = @employeeId)
                        UPDATE [HumanResources].[vEmployee]
                            SET Name = @name, RegionId = @regionId
                            WHERE BusinessEntityID = @employeeId
                    ELSE
                        INSERT INTO [HumanResources].[vEmployee] (BusinessEntityID, Name, RegionId)
                        VALUES (@employeeId, @name, @regionId)";
                    cmd.Parameters.AddWithValue("@employeeId", employeeId);
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@regionId", regionId);
                    cmd.CommandTimeout = 60;

                    // Execute the command
                    cmd.ExecuteNonQuery();
                }
            });
        }
    }
}
