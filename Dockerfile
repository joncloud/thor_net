FROM microsoft/dotnet:2.0-sdk
MAINTAINER joncloud <jdberube@gmail.com>

WORKDIR /thornet

ADD ./ThorNet.sln /thornet/ThorNet.sln
ADD ./src/ThorNet/ThorNet.csproj /thornet/src/ThorNet/ThorNet.csproj
ADD ./src/ThorNet.Sample/ThorNet.Sample.csproj /thornet/src/ThorNet.Sample/ThorNet.Sample.csproj
ADD ./tests/ThorNet.UnitTests/ThorNet.UnitTests.csproj /thornet/tests/ThorNet.UnitTests/ThorNet.UnitTests.csproj
RUN ["/bin/bash", "-c", "dotnet restore ThorNet.sln"]

ADD . /thornet
RUN ["/bin/bash", "-c", "dotnet build ThorNet.sln"]

ENTRYPOINT dotnet test tests/ThorNet.UnitTests/ThorNet.UnitTests.csproj
