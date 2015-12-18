FROM debian:jessie
MAINTAINER joncloud <jdberube@gmail.com>

RUN apt-get update
RUN apt-get install -y unzip curl libunwind8 gettext libssl-dev libcurl4-gnutls-dev zlib1g libicu-dev uuid-dev
RUN curl -sSL https://raw.githubusercontent.com/aspnet/Home/dev/dnvminstall.sh | DNX_BRANCH=dev sh

RUN bash -c "source ~/.dnx/dnvm/dnvm.sh \
	&& dnvm install 1.0.0-rc1-update1 -r coreclr \
	&& dnvm use 1.0.0-rc1-update1 -r coreclr -p"

WORKDIR /app
COPY src/ src/
ENV APP_SOURCE /app

RUN bash -c "source ~/.dnx/dnvm/dnvm.sh \
	&& dnu restore $APP_SOURCE/src/ThorNet/project.json $APP_SOURCE/src/ThorNet.Terminal/project.json $APP_SOURCE/src/ThorNet.UnitTests/project.json \
	&& dnu build $APP_SOURCE/src/ThorNet.UnitTests/project.json --configuration coreclr"

ENTRYPOINT bash -c "source ~/.dnx/dnvm/dnvm.sh \
	&& dnx --project $APP_SOURCE/src/ThorNet.UnitTests/project.json test"
