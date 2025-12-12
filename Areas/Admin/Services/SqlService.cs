using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using Corno.Web.Areas.Admin.Services.Interfaces;
using ConfigurationManager = System.Configuration.ConfigurationManager;

namespace Corno.Web.Areas.Admin.Services;

public class SqlService : ISqlService
{

    #region -- Protected Methods --

    [HttpPost]
    public DataTable ExecuteQuery(string sqlQuery)
    {
        var dataTable = new DataTable();

        var connectionString = ConfigurationManager.ConnectionStrings["CornoContext"].ConnectionString;
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        using var command = new SqlCommand(sqlQuery, connection);
        var adapter = new SqlDataAdapter(command);
        adapter.Fill(dataTable);

        return dataTable;
    }


    /* using var connection = new SqlConnection(ApplicationGlobals.ConnectionString);
     await connection.OpenAsync();

     var query = dto.Query.Trim();
     var isSelect = query.StartsWith("select", StringComparison.OrdinalIgnoreCase);

     using var sqlCommand = new SqlCommand(query, connection);

     if (isSelect)
     {
         using var reader = await sqlCommand.ExecuteReaderAsync();
         var result = new List<Dictionary<string, object>>();
         if (result == null)
             throw new ArgumentNullException(nameof(result));

         while (await reader.ReadAsync())
         {
             var row = new Dictionary<string, object>();
             for (var i = 0; i < reader.FieldCount; i++)
                 row[reader.GetName(i)] = reader[i];
             result.Add(row);
         }

         return;
     }*/
}

#endregion





