#!/bin/bash
# WireMock Startup Script for Local Testing
# Usage: ./start-wiremock.sh

WIREMOCK_VERSION="3.3.1"
WIREMOCK_JAR="wiremock-standalone-${WIREMOCK_VERSION}.jar"
WIREMOCK_PORT=9091

echo "========================================"
echo "  WireMock Local Server Startup"
echo "========================================"

# Check if JAR exists, download if not
if [ ! -f "$WIREMOCK_JAR" ]; then
    echo "📥 WireMock JAR not found. Downloading..."
    echo "Downloading: https://repo1.maven.org/maven2/org/wiremock/wiremock-standalone/${WIREMOCK_VERSION}/${WIREMOCK_JAR}"

    curl -L -o "$WIREMOCK_JAR" "https://repo1.maven.org/maven2/org/wiremock/wiremock-standalone/${WIREMOCK_VERSION}/${WIREMOCK_JAR}"

    if [ $? -eq 0 ]; then
        echo "✅ Download complete!"
    else
        echo "❌ Download failed. Please download manually from:"
        echo "https://repo1.maven.org/maven2/org/wiremock/wiremock-standalone/${WIREMOCK_VERSION}/${WIREMOCK_JAR}"
        exit 1
    fi
fi

echo ""
echo "🚀 Starting WireMock on port ${WIREMOCK_PORT}..."
echo "📁 Framework will auto-upload files from WireMock/ folder"
echo "⏹️  Press Ctrl+C to stop"
echo "========================================"
echo ""

java -jar "$WIREMOCK_JAR" \
    --port $WIREMOCK_PORT \
    --verbose \
    --global-response-templating

echo ""
echo "WireMock stopped."
