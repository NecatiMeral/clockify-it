# A simpler and faster alternative to git-filter-branch for cleansing bad data out of your Git repository history
FROM openjdk:8-jre-alpine

ENV BFG_VERSION 1.14.0

RUN wget -q -O bfg.jar https://repo1.maven.org/maven2/com/madgag/bfg/${BFG_VERSION}/bfg-${BFG_VERSION}.jar

WORKDIR /tmp

ENTRYPOINT ["java", "-jar", "/bfg.jar"]

CMD ["--help"]