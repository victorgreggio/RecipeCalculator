#!/bin/bash
java -jar ../antlr4/antlr-4.13.2-complete.jar \
  -Dlanguage=CSharp \
  -listener \
  -visitor \
  -o ../Generated/ \
  -package RecipeCalculator.Engine.Parser \
  Formula.g4
