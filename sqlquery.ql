import csharp

from MethodAccess ma
where
  ma.getTarget().hasName("ExecuteReader") and
  exists(Argument arg |
    arg.getValue().getType().getASupertype*().hasQualifiedName("System.Data.SqlClient", "SqlCommand") and
    exists(StringLiteral sl, Argument arg2 |
      arg.getValue().toString().indexOf("SELECT * FROM") != -1 and
      arg.getValue().toString().indexOf("WHERE") != -1 and
      arg.getValue().toString().indexOf("=") != -1 and
      arg.getValue().toString().indexOf("'") != -1 and
      sl.getValue().getParent*().getASupertype*().hasQualifiedName("Microsoft.AspNetCore.Mvc", "ControllerBase") and
      arg2.getValue().getParent*().getASupertype*().hasQualifiedName("Microsoft.AspNetCore.Mvc", "ControllerBase")
    )
  )
select ma, "Potential SQL Injection vulnerability: dynamic SQL query without parameterization"
