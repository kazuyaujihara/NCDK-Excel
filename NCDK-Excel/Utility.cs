﻿using NCDK;
using NCDK.Fingerprints;
using NCDK.IO;
using NCDK.QSAR;
using System;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Text;

namespace NCDKExcel
{
    public enum LineNotationType
    {
        Nil = 0,
        Smiles = 1,
        InChI = 2,
        MolText = 3,
    }

    public static class Utility
    {
        public const string SeparatorofNameKind = "(S-E-P-A-R-A-T-O-R)";

        public static string ToExcelString(object value)
        {
            switch (value)
            {
                case double f:
                    if (double.IsNaN(f))
                        return "#N/A";
                    if (double.IsInfinity(f))
                        return "#NUM!";
                    goto default;
                default:
                    return value.ToString();
            }
        }

        public static string ToExcelString(IDescriptorResult result)
        {
            return string.Join(", ", result.Values.Select(n => ToExcelString(n)));
        }

        public static string ToExcelString(IBitFingerprint fp)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < fp.Length; i++)
            {
                sb.Append(fp[i] ? "1" : "0");
            }
            return sb.ToString();
        }

        static IAtomContainer ParseSmiles(string smiles)
        {
            try
            {
                var mol = CDK.SmilesParser.ParseSmiles(smiles);
                if (mol.Atoms.Count == 0)
                    return null;
                return mol;
            }
            catch (Exception)
            {
            }
            return null;
        }

        static IAtomContainer ParseInChI(string inchi)
        {
            try
            {
                using (var reader = new InChIPlainTextReader(new StringReader(inchi)))
                {
                    var chemFile = reader.Read(CDK.Builder.NewChemFile());
                    if (chemFile.Count != 1)
                        throw new Exception();
                    var seq = chemFile[0];
                    if (seq.Count != 1)
                        throw new Exception();
                    var model = seq[0];
                    if (model.MoleculeSet.Count != 1)
                        throw new Exception();
                    var mol = model.MoleculeSet[0];
                    return mol;
                }
            }
            catch (Exception)
            {
            }
            return null;
        }

        /// <summary>
        /// Add implicit hydrogens.
        /// </summary>
        /// <param name="container">Molecular to add hydrogens</param>
        public static void AddImplicitHydrogens(IAtomContainer container)
        {
            var matcher = CDK.AtomTypeMatcher;
            int atomCount = container.Atoms.Count;
            string[] originalAtomTypeNames = new string[atomCount];
            for (int i = 0; i < atomCount; i++)
            {
                var atom = container.Atoms[i];
                var type = matcher.FindMatchingAtomType(container, atom);
                originalAtomTypeNames[i] = atom.AtomTypeName;
                atom.AtomTypeName = type.AtomTypeName;
            }
            var hAdder = CDK.HydrogenAdder;
            hAdder.AddImplicitHydrogens(container);
            // reset to the original atom types
            for (int i = 0; i < atomCount; i++)
            {
                var atom = container.Atoms[i];
                atom.AtomTypeName = originalAtomTypeNames[i];
            }
        }

        /// <summary>
        /// Dummy molecule to indicate <see langword="null"/>.
        /// </summary>
        static readonly IAtomContainer nullMol = CDK.Builder.NewAtomContainer();

        /// <summary>
        /// Parse <paramref name="text"/> as SMILES, InChI or MOL text and cache it.
        /// </summary>
        /// <param name="text">Text to parse</param>
        /// <returns>Molecular or <see langword="null"/> if it cannot be parsed.</returns>
        public static IAtomContainer Parse(string text)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));

            var cache = MemoryCache.Default;
            if (!(cache[text] is IAtomContainer mol))
            {
                mol = RawParse(text, out LineNotationType notationType);
                if (mol == null)
                    mol = nullMol;

                var policy = new CacheItemPolicy();
                cache.Set(text, mol, policy);
            }
            if (object.ReferenceEquals(mol, nullMol))
                return null;
            return mol;
        }

        static IAtomContainer RawParse(string text, out LineNotationType notationType)
        {
            IAtomContainer mol = null;

            if (text == null)
            {
                notationType = LineNotationType.Nil;
                return null;
            }
            if (text.IndexOf('\r') >= 0)
                goto Go_Mol;
            if (text.IndexOf('\n') >= 0)
                goto Go_Mol;

            mol = ParseSmiles(text);
            if (mol != null)
            {
                notationType = LineNotationType.Smiles;
                return mol;
            }

            mol = ParseInChI(text);
            if (mol != null)
            {
                notationType = LineNotationType.InChI;
                return mol;
            }

            Go_Mol:
            using (var r = new MDLV2000Reader(new StringReader(text)))
            {
                var m = CDK.Builder.NewAtomContainer();
                try
                {
                    r.Read(m);
                    mol = m;
                }
                catch (Exception)
                { }
            }
            if (mol != null)
            {
                notationType = LineNotationType.MolText;
                return mol;
            }

            using (var r = new MDLV3000Reader(new StringReader(text)))
            {
                var m = CDK.Builder.NewAtomContainer();
                try
                {
                    r.Read(m);
                    mol = m;
                }
                catch (Exception)
                { }
            }
            if (mol != null)
            {
                notationType = LineNotationType.MolText;
                return mol;
            }

            notationType = LineNotationType.Nil;
            return null;
        }
    }
}