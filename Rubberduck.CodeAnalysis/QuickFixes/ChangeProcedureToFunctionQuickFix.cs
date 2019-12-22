using System;
using System.Linq;
using Rubberduck.Inspections.Abstract;
using Rubberduck.Inspections.Concrete;
using Rubberduck.Parsing;
using Rubberduck.Parsing.Grammar;
using Rubberduck.Parsing.Inspections.Abstract;
using Rubberduck.Parsing.Rewriter;
using Rubberduck.Parsing.Symbols;

namespace Rubberduck.Inspections.QuickFixes
{
    public sealed class ChangeProcedureToFunctionQuickFix : QuickFixBase
    {
        public ChangeProcedureToFunctionQuickFix()
            : base(typeof(ProcedureCanBeWrittenAsFunctionInspection))
        {}

        public override void Fix(IInspectionResult result, IRewriteSession rewriteSession)
        {
            var parameterizedDeclaration = (IParameterizedDeclaration) result.Target;
            var arg = parameterizedDeclaration.Parameters.First(p => p.IsByRef || p.IsImplicitByRef);
            var argIndex = parameterizedDeclaration.Parameters.ToList().IndexOf(arg);
            
            UpdateSignature(result.Target, arg, rewriteSession);
            foreach (var reference in result.Target.References.Where(reference => !reference.IsDefaultMemberAccess))
            {
                UpdateCall(reference, argIndex, rewriteSession);
            }
        }

        public override string Description(IInspectionResult result) => Resources.Inspections.QuickFixes.ProcedureShouldBeFunctionInspectionQuickFix;

        private void UpdateSignature(Declaration target, ParameterDeclaration arg, IRewriteSession rewriteSession)
        {
            var subStmt = (VBAParser.SubStmtContext) target.Context;
            var argContext = (VBAParser.ArgContext)arg.Context;

            var rewriter = rewriteSession.CheckOutModuleRewriter(target.QualifiedModuleName);

            rewriter.Replace(subStmt.SUB(), Tokens.Function);
            rewriter.Replace(subStmt.END_SUB(), "End Function");

            rewriter.InsertAfter(subStmt.argList().Stop.TokenIndex, $" As {arg.AsTypeName}");

            if (arg.IsByRef)
            {
                rewriter.Replace(argContext.BYREF(), Tokens.ByVal);
            }
            else if (arg.IsImplicitByRef)
            {
                rewriter.InsertBefore(argContext.unrestrictedIdentifier().Start.TokenIndex, Tokens.ByVal);
            }

            var returnStmt = $"    {subStmt.subroutineName().GetText()} = {argContext.unrestrictedIdentifier().GetText()}{Environment.NewLine}";
            rewriter.InsertBefore(subStmt.END_SUB().Symbol.TokenIndex, returnStmt);
        }

        private void UpdateCall(IdentifierReference reference, int argIndex, IRewriteSession rewriteSession)
        {
            var rewriter = rewriteSession.CheckOutModuleRewriter(reference.QualifiedModuleName);
            var callStmtContext = reference.Context.GetAncestor<VBAParser.CallStmtContext>();
            var argListContext = callStmtContext.GetChild<VBAParser.ArgumentListContext>();

            var arg = argListContext.argument()[argIndex];
            var argName = arg.positionalArgument()?.argumentExpression() ?? arg.namedArgument().argumentExpression();

            rewriter.InsertBefore(callStmtContext.Start.TokenIndex, $"{argName.GetText()} = ");
            rewriter.Replace(callStmtContext.whiteSpace(), "(");
            rewriter.InsertAfter(argListContext.Stop.TokenIndex, ")");
        }

        public override bool CanFixInProcedure => false;
        public override bool CanFixInModule => false;
        public override bool CanFixInProject => false;
    }
}