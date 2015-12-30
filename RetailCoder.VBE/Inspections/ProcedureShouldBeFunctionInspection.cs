﻿using System.Collections.Generic;
using System.Linq;
using Rubberduck.Parsing;
using Rubberduck.Parsing.Grammar;
using Rubberduck.Parsing.VBA;

namespace Rubberduck.Inspections
{
    public class ProcedureShouldBeFunctionInspection : IInspection
    {
        public ProcedureShouldBeFunctionInspection()
        {
            Severity = CodeInspectionSeverity.Warning;
        }

        public string Name { get { return "ProcedureShouldBeFunctionInspection"; } }
        public string Description { get { return InspectionsUI.ProcedureShouldBeFunctionInspection; } }
        public CodeInspectionType InspectionType { get { return CodeInspectionType.LanguageOpportunities; } }
        public CodeInspectionSeverity Severity { get; set; }

        public IEnumerable<CodeInspectionResultBase> GetInspectionResults(RubberduckParserState state)
        {
            return state.ArgListsWithOneByRefParam
                .Where(context => context.Context.Parent is VBAParser.SubStmtContext)
                .Select(context => new ProcedureShouldBeFunctionInspectionResult(this,
                    new QualifiedContext<VBAParser.SubStmtContext>(context.ModuleName, context.Context.Parent as VBAParser.SubStmtContext)));
        }
    }
}
