NCDK-Excel: Cheminformatics function for Excel
==============================================

NCDK-Excel is Add-in for enabling cheminformatics functions in Excel worksheet.

Getting Started
---------------

Input a formula beginning with "NCDK_", "RDKit_", or "OPSIN_" in the Excel cell like the followings.

| Formula  | Output |    |
| ---- | ---- | ---- |
| `=NCDK_ECFP4("c1ccccc1C")` | 0000...0000 | ECFP4 of toluene created by NCDK |
| `=NCDK_XLogP("c1ccccc1C")` | 2.459 | XLogP calculated by NCDK |
| `=RDKit_MACCSFingerprint("c1ccccc1C")` | 0000...1010 | MACCS fingerprint created by RDKit |
| `=RDKit_MolBlock("c1ccccc1C")` | "RDKit ... M  END" | MolBlock of toluene created by RDKit |
| `=NCDK_SMILES("RDKit ... M  END")` | C1=CC=CC=C1C | SMILES of MolBlock expression of toluene crated by NCDK |
| `=RDKit_Smiles("c1c(C)cccc1")` | Cc1ccccc1 | SMILES of toluene |
| `=RDKit_Smiles("c1ccccc1", , TRUE)`| C1=CC=CC=C1 | Kekulize SMILES of benzene |
| `=NCDK_InChI("c1ccccc1C")` | "InChI=1S/C7H8/c1-7-5-3-2-4-6-7/h2-6H,1H3" | InChI created by NCDK |
| `=NCDK_Tanimoto(NCDK_ECFP4("c1ccccc1"), NCDK_ECFP4("c1(Cl)ccc(C)cc1"))` | 0.07 | Tanimoto smilarity between benzene and 4-chlorotoluene based on ECFP4 |
| `=OPSIN_ParseToSmiles("toluene")` | "CC1=CC=CC=C1" | SMILES of toluene |
| `=OPSIN_ParseToStdInChIKey("benzene")` | UHOVQNZJYSORNB-UHFFFAOYSA-N | InChIKey of benzene |

Has Substructure

- `=RDKit_HasSubstructMatch("CC(C)CCCC(C)C1CCC2C1(CCC3C2CC=C4C3(CCC(C4)O)C)C", "C1CCCCC1")` → TRUE
- `=RDKit_HasSubstructMatch("CC(C)CCCC(C)C1CCC2C1(CCC3C2CC=C4C3(CCC(C4)O)C)C", "c1ccccc1")` → FALSE

Run Reaction SMILES

- `=RDKit_RunReactionSmiles("[C:1]>>[C:1]C", "Cc1ccccc1")` → "CCc1ccccc1"
- `=RDKit_RunReactionSmiles("[c:1]>>[c:1]C", "Cc1ccccc1")` → "Cc1(Cl)ccccc1"
- `=RDKit_RunReactionSmiles("[c:1][Cl:2].[C:3]B>>[c:1][C:3]", "c1ccccc1Cl.BCCCC")` → "CCCCc1ccccc1"

Impoting SDF

- Click "Import SDF" button in Add-ins tab in ribbon and select SD file. It gives a new worksheet imported the SDF.

Generate picture from SMILES/InChI/MOL block

- Select cells and click "Generate Picture".
- Picture format, color, and toolkit can be selected in `Preference`.

Screenshot
----------

![screenshot](image/NCDK-Excel-Worksheet-1.png?raw=true)

How to Install
--------------

Execute `NCDK-Excel-x86.msi` or `NCDK-Excel-x64.msi` and launch Excel.
If you want to enable or disable it, go "File" > "Options" > "Add-ins" > "Manage: COM-Add-ins" > "GO..." and check/uncheck 'NCDK for Excel'.

Build
-----

Open the solution file, `NCDK-Excel.sln`, by Visual Studio 2019 and `Build Solution` to build it.
Installers are generated in `Setup\Release` and `Setup64\Release` folder on `Release` configuration.

NCDK functions
--------------

AcidicGroupCount
APol
AromaticAtomsCount
AromaticBondsCount
AtomCount
AutocorrelationCharge
AutocorrelationMass
AutocorrelationPolarizability
BasicGroupCount
BCUT
BondCount
BPol
CarbonTypes
ChiChain
ChiCluster
ChiPathCluster
ChiPath
CPSA
EccentricConnectivityIndex
FMF
FractionalPSA
FractionalCSP3
FSP3
FragmentComplexity
GravitationalIndex
HBondAcceptorCount
HBondDonorCount
HybridizationRatio
JPlogP
KappaShapeIndices
KierHallSmarts
LargestChain
LargestPiSystem
LengthOverBreadth
LongestAliphaticChain
MannholdLogP
MDE
MomentOfInertia
PetitjeanNumber
PetitjeanShapeIndex
RotatableBondsCount
RuleOfFive
SmallRing
TPSA
VABC
VAdjMa
WeightedPath
WHIM
WienerNumbers
XLogP
ZagrebIndex
ECFP0
ECFP2
ECFP4
ECFP6
FCFP0
FCFP2
FCFP4
FCFP6
ALogP
AMolarRefractivity
MolecularWeight
ExactMas
SMILES
InChI
InChIKey
MolBlock
Tanimoto

RDKit Functions
---------------

MACCSFingerprint
LayeredFingerprint
HashedAtomPairFingerprint
HashedTopologicalTorsionFingerprint
PatternFingerprint
RDKFingerprint
AtomPairFingerprint
TopologicalTorsionFingerprint
Chi0n
Chi0v
Chi1n
Chi1v
Chi2n
Chi2v
Chi3n
Chi3v
Chi4n
Chi4v
Kappa1
Kappa2
Kappa3
LabuteASA
MolLogP
MolMR
ExactMW
FractionCSP3
HallKierAlpha
LipinskiHBA
LipinskiHBD
NumAliphaticCarbocycles
NumAliphaticHeterocycles
NumAliphaticRings
NumAmideBonds
NumAromaticCarbocycles
NumAromaticHeterocycles
NumAromaticRings
NumBridgeheadAtoms
NumHBA
NumHBD
TPSA
NumRotatableBonds
NumHeteroatoms
NumHeterocycles
NumRings
NumSaturatedCarbocycles
NumSaturatedHeterocycles
NumSaturatedRings
NumSpiroAtoms
NumAtoms
NumBonds
NumHeavyAtoms
BalabanJ
FormalCharge
AtomStereoCenters
UnspecifiedAtomStereoCenters
MolFormula
CXSmiles
InchiKey
MolBlock
PDBBlock
TPLBlock
XYZBlock
Smarts
Smiles
TanimotoSimilarity
AllBitSimilarity
AsymmetricSimilarity
BraunBlanquetSimilarity
CosineSimilarity
DiceSimilarity
KulczynskiSimilarity
McConnaugheySimilarity
OnBitSimilarity
RusselSimilarity
SokalSimilarity
TverskySimilarity
StripMol

OPSIN functions
---------------

ParseToSmiles
ParseToCML

Copyright (c) 2018-2021 Kazuya Ujihara
