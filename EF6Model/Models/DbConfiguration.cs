using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EF6Model.Models
{
  public class NinjaDbConfiguration : DbConfiguration
  {
    public NinjaDbConfiguration() {
      SetManifestTokenResolver(new MyManifestTokenResolver());
    }

    public class MyManifestTokenResolver : IManifestTokenResolver
    {
      private readonly IManifestTokenResolver _defaultResolver = new DefaultManifestTokenResolver();

      public string ResolveManifestToken(DbConnection connection) {
        var sqlConn = connection as SqlConnection;
        if (sqlConn != null) {
          return "2012";
        }
        else {
          return _defaultResolver.ResolveManifestToken(connection);
        }
      }
    }
  }
}
