#!/bin/bash

dotnet test --filter FullyQualifiedName~LoiTasks.Tests."$1"
