﻿using ExcelDna.Integration;
using GraphMolWrap;
using RDKit;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace NCDKExcel
{
    public static partial class RDKit_ReactionFunctions
    {
        static readonly ConcurrentDictionary<Tuple<string, string>, string> RunReactionCache =
            new ConcurrentDictionary<Tuple<string, string>, string>();

        [ExcelFunction(Description = "Apply reaction SMILES to the reactant(s).")]
        public static string RDKit_RunReactionSmiles(string rxnIdent, string reactantsIdent)
        {
            var key = new Tuple<string, string>(rxnIdent, reactantsIdent);
            var result = RunReactionCache.GetOrAdd(key, tuple =>
            {
                var a_rxnIdent = tuple.Item1;
                var a_reactantsIdent = tuple.Item2;

                try
                {
                    var rxn = RDKitChemicalReaction.Parse(a_rxnIdent);
                    var mols = new List<ROMol>();
                    foreach (var reactant_smiles in a_reactantsIdent.Split('.'))
                    {
                        var mol = RDKitMol.Parse(reactant_smiles);
                        if (mol == null)
                            return null;
                        mol = mol.AddHs();
                        mols.Add(mol);
                    }

                    if (rxn.getNumReactantTemplates() != mols.Count)
                        return null;

                    var reactants = new ROMol_Vect(mols);
                    var products_candidate_list = rxn.runReactants(reactants);
                    if (products_candidate_list.Count == 0)
                        return null;

                    return string.Join(".", products_candidate_list.First().Select(mol => Chem.MolToSmiles(Chem.RemoveHs(mol))));
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            });

            return result;
        }

        [ExcelFunction(Description = "Remove salt.")]
        public static string RDKit_StripMol(string smiles)
        {
            var stripped = Chem.StaticSaltRemover.StripMol(RDKitMol.Parse(smiles));
            return Chem.MolToSmiles(stripped);
        }
    }

    public static class RDKitChemicalReaction
    {
        static readonly ConcurrentDictionary<string, ChemicalReaction> ReactionCache = new ConcurrentDictionary<string, ChemicalReaction>();
        static readonly ChemicalReaction nullRxn = new ChemicalReaction();

        public static ChemicalReaction Parse(string rxnSmarts)
        {
            if (rxnSmarts == null)
                throw new ArgumentNullException(nameof(rxnSmarts));

            var rxn = ReactionCache.GetOrAdd(rxnSmarts, a_rxnSmarts =>
            {
                var a_rxn = ChemicalReaction.ReactionFromSmarts(a_rxnSmarts);
                if (a_rxn == null)
                    return nullRxn;

                a_rxn.setProp("source", "Reaction SMILES");
                return a_rxn;
            });
            if (object.ReferenceEquals(rxn, nullRxn))
                return null;

            return rxn;
        }
    }
}
