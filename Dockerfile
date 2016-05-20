FROM microsoft/dotnet:onbuild
MAINTAINER joncloud <jdberube@gmail.com>

WORKDIR /dotnetapp/src/ThorNet.UnitTests/
ENTRYPOINT dotnet test
