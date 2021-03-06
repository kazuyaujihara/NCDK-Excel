﻿/*
 * MIT License
 * 
 * Copyright (c) 2019-2020 Kazuya Ujihara
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using ExcelDna.Integration;
using GraphMolWrap;
using RDKit;
using System;
using System.Collections;
using System.Collections.Generic;

namespace NCDKExcel
{
    public static class DistanceFunctions
    {
        [ExcelFunction(Description = "Returns the Tanimoto similariry.")]
        public static double NCDK_Tanimoto(string fp1, string fp2)
        {
            var val = double.NaN;
            var ba1 = ToBitArrayOrNull(fp1);
            var ba2 = ToBitArrayOrNull(fp2);

            if (ba1 != null && ba2 != null)
            {
                if (ba1.Length == ba2.Length)
                {
                    val = NCDK.Similarity.Tanimoto.Calculate(ba1, ba2);
                }
            }

            return val;
        }

        /// <summary>
        /// Convert <paramref name="fp"/> to <see cref="BitArray"/> or <see langword="null"/> if failed.
        /// </summary>
        /// <param name="fp">Fingerprint expressed in string to convert.</param>
        /// <returns>Fingerprint in <see cref="BitArray"/>, <see langword="null"/> on failed to convert.</returns>
        public static BitArray ToBitArrayOrNull(string fp)
        {
            var array = new bool[fp.Length];
            for (int i = 0; i < fp.Length; i++)
            {
                switch (fp[i])
                {
                    case '0':
                        array[i] = false;
                        break;
                    case '1':
                        array[i] = true;
                        break;
                    default:
                        return null;
                }
            }
            return new BitArray(array);
        }

        [ExcelFunction(Description = "Returns the Tanimoto similariry.")]
        public static double RDKit_TanimotoSimilarity(string fp1, string fp2, bool returnDistance, double bounds)
        {
            var val = RDKit_Similarity(DataStructs.TanimotoSimilarity, fp1, fp2);
            if (double.IsNaN(val))
                val = RDKit_SimilaritySIV64(DataStructs.TanimotoSimilarity, fp1, fp2, returnDistance, bounds);
            return val;
        }

        [ExcelFunction(Description = "Returns the dice similariry.")]
        public static double RDKit_DiceSimilarity(string fp1, string fp2, bool returnDistance, double bounds)
        {
            var val = RDKit_Similarity(DataStructs.DiceSimilarity, fp1, fp2);
            if (double.IsNaN(val))
                val = RDKit_SimilaritySIV64(DataStructs.DiceSimilarity, fp1, fp2, returnDistance, bounds);
            return val;
        }

        [ExcelFunction(Description = "Returns the cosine similariry.")]
        public static double RDKit_CosineSimilarity(string fp1, string fp2)
        {
            return RDKit_Similarity(DataStructs.CosineSimilarity, fp1, fp2);
        }

        [ExcelFunction(Description = "Returns the Sokal similariry.")]
        public static double RDKit_SokalSimilarity(string fp1, string fp2)
        {
            return RDKit_Similarity(DataStructs.SokalSimilarity, fp1, fp2);
        }

        [ExcelFunction(Description = "Returns the Russel similariry.")]
        public static double RDKit_RusselSimilarity(string fp1, string fp2)
        {
            return RDKit_Similarity(DataStructs.RusselSimilarity, fp1, fp2);
        }

        //[ExcelFunction()]
        //public static double RDKit_RogotGoldbergSimilarity(string fp1, string fp2)
        //{
        //    return RDKit_Similarity(DataStructs.RogotGoldbergSimilarity, fp1, fp2);
        //}

        [ExcelFunction(Description = "Returns the all bit similariry.")]
        public static double RDKit_AllBitSimilarity(string fp1, string fp2)
        {
            return RDKit_Similarity(DataStructs.AllBitSimilarity, fp1, fp2);
        }

        [ExcelFunction(Description = "Returns the Kulczynski similariry.")]
        public static double RDKit_KulczynskiSimilarity(string fp1, string fp2)
        {
            return RDKit_Similarity(DataStructs.KulczynskiSimilarity, fp1, fp2);
        }

        [ExcelFunction(Description = "Returns the McConnaughey similariry.")]
        public static double RDKit_McConnaugheySimilarity(string fp1, string fp2)
        {
            return RDKit_Similarity(DataStructs.McConnaugheySimilarity, fp1, fp2);
        }

        [ExcelFunction(Description = "Returns the asymmetric similariry.")]
        public static double RDKit_AsymmetricSimilarity(string fp1, string fp2)
        {
            return RDKit_Similarity(DataStructs.AsymmetricSimilarity, fp1, fp2);
        }

        [ExcelFunction(Description = "Returns the Braun Blanquet similariry.")]
        public static double RDKit_BraunBlanquetSimilarity(string fp1, string fp2)
        {
            return RDKit_Similarity(DataStructs.BraunBlanquetSimilarity, fp1, fp2);
        }

        [ExcelFunction(Description = "Returns the Tversky similariry.")]
        public static double RDKit_TverskySimilarity(string fp1, string fp2, double a, double b, bool returnDistance, double bounds)
        {
            var val = RDKit_Similarity((ExplicitBitVect v1, ExplicitBitVect v2) => DataStructs.TverskySimilarity(v1, v2, a, b), fp1, fp2);
            if (double.IsNaN(val))
                val = RDKit_SimilaritySIV64((SparseIntVect64 v1, SparseIntVect64 v2, bool _returnDistance, double _bounds) => DataStructs.TverskySimilarity(v1, v2, a, b, _returnDistance, _bounds), fp1, fp2, returnDistance, bounds);
            return val;
        }

        static double RDKit_Similarity(Func<ExplicitBitVect, ExplicitBitVect, double> func, string fp1, string fp2)
        {
            var val = double.NaN;
            var bv1 = ToBitVectorOrNull(fp1);
            var bv2 = ToBitVectorOrNull(fp2);

            if (bv1 != null && bv2 != null)
            {
                if (bv1.size() == bv2.size())
                {
                    val = func(bv1, bv2);
                }
            }

            return val;
        }

        static double RDKit_SimilaritySIV64(Func<SparseIntVect64, SparseIntVect64, bool, double, double> func, string fp1, string fp2, bool returnDistance, double bounds)
        {
            var val = double.NaN;
            try
            {
                var sv1 = RDKitSparseVector.FromJsonToSIV(fp1);
                var sv2 = RDKitSparseVector.FromJsonToSIV(fp2);
                
                val = func(sv1, sv2, returnDistance, bounds);
            }
            catch (Exception)
            {
            }

            return val;
        }

        public static ExplicitBitVect ToBitVectorOrNull(string fp)
        {
            var array = new List<int>();
            for (int i = 0; i < fp.Length; i++)
            {
                switch (fp[i])
                {
                    case '0':
                        break;
                    case '1':
                        array.Add(i);
                        break;
                    default:
                        return null;
                }
            }
            var o = new ExplicitBitVect((uint)fp.Length);
            foreach (var i in array)
            {
                o.setBit((uint)i);
            }
            return o;
        }
    }
}
