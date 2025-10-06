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