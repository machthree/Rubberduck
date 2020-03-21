﻿using System.Collections.Generic;
using Antlr4.Runtime;
using Rubberduck.Parsing.Annotations;
using Rubberduck.Parsing.Binding;
using Rubberduck.Parsing.Grammar;
using Rubberduck.VBEditor;

namespace Rubberduck.Parsing.Symbols
{
    public class ArgumentReference : IdentifierReference
    {
        public ArgumentReference(
            QualifiedModuleName qualifiedName,
            Declaration parentScopingDeclaration,
            Declaration parentNonScopingDeclaration,
            string identifierName,
            Selection argumentSelection,
            ParserRuleContext context,
            VBAParser.ArgumentListContext argumentListContext,
            ArgumentListArgumentType argumentType,
            int argumentPosition,
            ParameterDeclaration referencedParameter,
            IEnumerable<IParseTreeAnnotation> annotations = null)
            : base(
                qualifiedName,
                parentScopingDeclaration,
                parentNonScopingDeclaration,
                identifierName,
                argumentSelection,
                context,
                referencedParameter,
                false,
                false,
                annotations)
        {
            ArgumentType = argumentType;
            ArgumentPosition = argumentPosition;
            ArgumentListContext = argumentListContext;
            NumberOfArguments = ArgumentListContext?.argument()?.Length ?? 0;
            ArgumentListSelection = argumentListContext?.GetSelection() ?? Selection.Empty;
        }

        public ArgumentListArgumentType ArgumentType { get; }
        public int ArgumentPosition { get; }
        public int NumberOfArguments { get; }
        public VBAParser.ArgumentListContext ArgumentListContext { get; }
        public Selection ArgumentListSelection { get; }
    }
}