# Setup

## Create Projects & Solution

dotnet new sln -n ParseAndRecreation

md FlowParser
cd FlowParser
dotnet new classlib -n FlowParser --framework net8.0
dotnet new xunit -n FlowParser.Tests --framework net8.0
dotnet add FlowParser.Tests/FlowParser.Tests.csproj reference FlowParser/FlowParser.csproj

cd ..
dotnet sln add FlowParser/FlowParser/FlowParser.csproj
dotnet sln add FlowParser/FlowParser.Tests/FlowParser.Tests.csproj


md RegularParser
cd RegularParser
dotnet new classlib -n RegularParser --framework net8.0
dotnet new xunit -n RegularParser.Tests --framework net8.0
dotnet add RegularParser.Tests/RegularParser.Tests.csproj reference RegularParser/RegularParser.csproj

cd ..
dotnet sln add RegularParser/RegularParser/RegularParser.csproj
dotnet sln add RegularParser/RegularParser.Tests/RegularParser.Tests.csproj


dotnet new classlib -n Definitions --framework net8.0
dotnet sln add Definitions/Definitions.csproj
dotnet add RegularParser/RegularParser/RegularParser.csproj reference Definitions/Definitions.csproj
dotnet add FlowParser/FlowParser/FlowParser.csproj reference Definitions/Definitions.csproj
