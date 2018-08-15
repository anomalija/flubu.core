using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using TestHelper;
using Xunit;

namespace FlubuCore.Analyzers.Tests
{
    public class TargetParameterAnalyzerUnitTests : CodeFixVerifier
    {
        [Fact]
        public void CorrectTargetDefinititionTest()
        {
            var test = @"
using System;
using System.IO;
using FlubuCore.Context;
using FlubuCore.Context.FluentInterface;
using FlubuCore.Context.FluentInterface.Interfaces;
using FlubuCore.Scripting;
using FlubuCore.Targeting;
using Moq;

namespace FlubuCore.WebApi.Tests
{
   public class Target : System.Attribute
    {
        public TargetAttribute(string targetName, params object[] methodParameters)
        {
            TargetName = targetName;
            MethodParameters = methodParameters;
        }

        public string TargetName { get; private set; }

        public object[] MethodParameters { get; set; }
    }

    public class SimpleScript : DefaultBuildScript
    {
        protected override void ConfigureBuildProperties(IBuildPropertiesContext context)
        {
        }

        protected override void ConfigureTargets(ITaskContext session)
        {
        }

        [Target(""Test"", ""SomeFile"")]
        public void SuccesfullTarget(ITarget target, string fileName)
        {
        }
     }
}";
            VerifyCSharpDiagnostic(test);
        }

        [Fact]
        public void WrongFirstParameterTest()
        {
            var test = @"
  using System;
using System.IO;
using FlubuCore.Context;
using FlubuCore.Context.FluentInterface;
using FlubuCore.Context.FluentInterface.Interfaces;
using FlubuCore.Scripting;
using FlubuCore.Targeting;
using Moq;

namespace FlubuCore.WebApi.Tests
{
    public class SimpleScript : DefaultBuildScript
    {
        protected override void ConfigureBuildProperties(IBuildPropertiesContext context)
        {
        }

        protected override void ConfigureTargets(ITaskContext session)
        {
        }

        [Target(\""Test\"")]
        public void SuccesfullTarget(string fileName)
        {
        }
     }
}";
            var expected = new DiagnosticResult
            {
                Id = "FlubuCore_TargetParameter_001",
                Message = String.Format("First parameter in method '{0}' must be of type ITarget.", "SuccesfullTarget"),
                Severity = DiagnosticSeverity.Error,
                Locations =
                    new[] {
                        new DiagnosticResultLocation("Test0.cs", 24, 21)
                    }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [Fact]
        public void NoFirstParameterTest()
        {
            var test = @"
  using System;
using System.IO;
using FlubuCore.Context;
using FlubuCore.Context.FluentInterface;
using FlubuCore.Context.FluentInterface.Interfaces;
using FlubuCore.Scripting;
using FlubuCore.Targeting;
using Moq;

namespace FlubuCore.WebApi.Tests
{
    public class SimpleScript : DefaultBuildScript
    {
        protected override void ConfigureBuildProperties(IBuildPropertiesContext context)
        {
        }

        protected override void ConfigureTargets(ITaskContext session)
        {
        }

        [Target((""Test"")]
        public void SuccesfullTarget()
        {
        }
     }
}";
            var expected = new DiagnosticResult
            {
                Id = "FlubuCore_TargetParameter_001",
                Message = String.Format("First parameter in method '{0}' must be of type ITarget.", "SuccesfullTarget"),
                Severity = DiagnosticSeverity.Error,
                Locations =
                    new[] {
                        new DiagnosticResultLocation("Test0.cs", 24, 21)
                    }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [Fact]
        public void WrongParameterCountTest()
        {
            var test = @"
using System;
using System.IO;
using FlubuCore.Context;
using FlubuCore.Context.FluentInterface;
using FlubuCore.Context.FluentInterface.Interfaces;
using FlubuCore.Scripting;
using FlubuCore.Targeting;
using Moq;

namespace FlubuCore.WebApi.Tests
{
    public class Target : System.Attribute
    {
        public TargetAttribute(string targetName, params object[] methodParameters)
        {
            TargetName = targetName;
            MethodParameters = methodParameters;
        }

        public string TargetName { get; private set; }

        public object[] MethodParameters { get; set; }
    }

    public class SimpleScript : DefaultBuildScript
    {
        protected override void ConfigureBuildProperties(IBuildPropertiesContext context)
        {
        }

        protected override void ConfigureTargets(ITaskContext session)
        {
        }

        [Target(""Test"", ""param1"", ""param2"", ""param3"")]
        public void SuccesfullTarget(ITarget target, string fileName, string path)
        {
        }
     }
}";
            var expected = new DiagnosticResult
            {
                Id = "FlubuCore_TargetParameter_002",
                Message = String.Format("Parameters count in attribute and  method '{0}' must be the same.", "SuccesfullTarget"),
                Severity = DiagnosticSeverity.Error,
                Locations =
                    new[] {
                        new DiagnosticResultLocation("Test0.cs", 36, 10)
                    }
            };

            VerifyCSharpDiagnostic(test, expected);
        }


        [Fact]
        public void AttributeDoesntHaveParametersMethodDoesTest()
        {
            var test = @"
using System;
using System.IO;
using FlubuCore.Context;
using FlubuCore.Context.FluentInterface;
using FlubuCore.Context.FluentInterface.Interfaces;
using FlubuCore.Scripting;
using FlubuCore.Targeting;
using Moq;

namespace FlubuCore.WebApi.Tests
{
    public class Target : System.Attribute
    {
        public TargetAttribute(string targetName, params object[] methodParameters)
        {
            TargetName = targetName;
            MethodParameters = methodParameters;
        }

        public string TargetName { get; private set; }

        public object[] MethodParameters { get; set; }
    }

    public class SimpleScript : DefaultBuildScript
    {
        protected override void ConfigureBuildProperties(IBuildPropertiesContext context)
        {
        }

        protected override void ConfigureTargets(ITaskContext session)
        {
        }

        [Target(""Test"")]
        public void SuccesfullTarget(ITarget target, string fileName)
        {
        }
     }
}";
            var expected = new DiagnosticResult
            {
                Id = "FlubuCore_TargetParameter_002",
                Message = String.Format("Parameters count in attribute and  method '{0}' must be the same.", "SuccesfullTarget"),
                Severity = DiagnosticSeverity.Error,
                Locations =
                    new[] {
                        new DiagnosticResultLocation("Test0.cs", 36, 10)
                    }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [Fact]
        public void AttributeAndMethodParameterCountAreTheSameTest()
        {
            var test = @"
using System;
using System.IO;
using FlubuCore.Context;
using FlubuCore.Context.FluentInterface;
using FlubuCore.Context.FluentInterface.Interfaces;
using FlubuCore.Scripting;
using FlubuCore.Targeting;
using Moq;

namespace FlubuCore.WebApi.Tests
{
    public class Target : System.Attribute
    {
        public TargetAttribute(string targetName, params object[] methodParameters)
        {
            TargetName = targetName;
            MethodParameters = methodParameters;
        }

        public string TargetName { get; private set; }

        public object[] MethodParameters { get; set; }
    }

    public class SimpleScript : DefaultBuildScript
    {
        protected override void ConfigureBuildProperties(IBuildPropertiesContext context)
        {
        }

        protected override void ConfigureTargets(ITaskContext session)
        {
        }

        [Target(""Test"" , ""someFilename"")]
        public void SuccesfullTarget(ITarget target, string fileName)
        {
        }
     }
}";
            var expected = new DiagnosticResult
            {
                Id = "FlubuCore_TargetParameterAnalyzer",
                Message = String.Format("Parameters count in attribute and  method '{0}' must be the same.", "SuccesfullTarget"),
                Severity = DiagnosticSeverity.Error,
                Locations =
                    new[] {
                        new DiagnosticResultLocation("Test0.cs", 36, 10)
                    }
            };

            VerifyCSharpDiagnostic(test);
        }

        [Fact]
        public void WrongParameterTypeTest()
        {
            var test = @"
using System;
using System.IO;
using FlubuCore.Context;
using FlubuCore.Context.FluentInterface;
using FlubuCore.Context.FluentInterface.Interfaces;
using FlubuCore.Scripting;
using FlubuCore.Targeting;
using Moq;

namespace FlubuCore.WebApi.Tests
{
    public class Target : System.Attribute
    {
        public TargetAttribute(string targetName, params object[] methodParameters)
        {
            TargetName = targetName;
            MethodParameters = methodParameters;
        }

        public string TargetName { get; private set; }

        public object[] MethodParameters { get; set; }
    }

    public class SimpleScript : DefaultBuildScript
    {
        protected override void ConfigureBuildProperties(IBuildPropertiesContext context)
        {
        }

        protected override void ConfigureTargets(ITaskContext session)
        {
        }

        [Target(""Test"", ""param1"", 1)]
        public void SuccesfullTarget(ITarget target, string fileName, string path)
        {
        }
     }
}";
            var expected = new DiagnosticResult
            {
                Id = "FlubuCore_TargetParameter_003",
                Message = String.Format("Parameter must be of same type as '{0}' method parameter '{1}'.", "SuccesfullTarget",  "path"),
                Severity = DiagnosticSeverity.Error,
                Locations =
                    new[] {
                        new DiagnosticResultLocation("Test0.cs", 36, 35)
                    }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new TargetParameterAnalyzer();
        }
    }
}