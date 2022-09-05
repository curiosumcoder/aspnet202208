using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable

namespace CA40.Data
{
    public partial class NWContext
    {
        public virtual DbSet<SalesbyYearResult> SalesbyYearResults { get; set; }

        public List<SalesbyYearResult> SalesByYear(DateTime? start, DateTime? end)
        {
            var parameters = new[]
            {
                new SqlParameter
                {
                    ParameterName = "Beginning_Date",
                    Value = start ?? Convert.DBNull,
                    SqlDbType = System.Data.SqlDbType.DateTime,
                },
                new SqlParameter
                {
                    ParameterName = "Ending_Date",
                    Value = end ?? Convert.DBNull,
                    SqlDbType = System.Data.SqlDbType.DateTime,
                },
            };

            var sql = "EXEC [dbo].[Sales by Year] @Beginning_Date, @Ending_Date";

            return this.Set<SalesbyYearResult>().FromSqlRaw(sql, parameters).ToList();
        }
    }
}
