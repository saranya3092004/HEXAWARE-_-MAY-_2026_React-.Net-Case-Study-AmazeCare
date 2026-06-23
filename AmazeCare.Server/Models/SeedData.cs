using AmazeCare.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace AmazeCare.Server.Data
{
    public static class SeedData
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            SeedMedicines(modelBuilder);
            SeedLabTestCatalog(modelBuilder);
        }

        // ─── Medicines ────────────────────────────────────────────────────────────
        private static void SeedMedicines(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Medicine>().HasData(

                // Antipyretics / Analgesics
                new Medicine { MedicineId = 1, Name = "Paracetamol 500mg", GenericName = "Acetaminophen", Category = MedicineCategory.Antipyretic, DefaultDosage = "500mg",  IsActive = true },
                new Medicine { MedicineId = 2, Name = "Paracetamol 650mg", GenericName = "Acetaminophen", Category = MedicineCategory.Antipyretic, DefaultDosage = "650mg",  IsActive = true },
                new Medicine { MedicineId = 3, Name = "Ibuprofen 400mg", GenericName = "Ibuprofen", Category = MedicineCategory.Analgesic, DefaultDosage = "400mg",  IsActive = true },
                new Medicine { MedicineId = 4, Name = "Diclofenac 50mg", GenericName = "Diclofenac Sodium", Category = MedicineCategory.Analgesic, DefaultDosage = "50mg",  IsActive = true },

                // Antibiotics
                new Medicine { MedicineId = 5, Name = "Amoxicillin 250mg", GenericName = "Amoxicillin", Category = MedicineCategory.Antibiotic, DefaultDosage = "250mg",  IsActive = true },
                new Medicine { MedicineId = 6, Name = "Amoxicillin 500mg", GenericName = "Amoxicillin", Category = MedicineCategory.Antibiotic, DefaultDosage = "500mg",  IsActive = true },
                new Medicine { MedicineId = 7, Name = "Azithromycin 250mg", GenericName = "Azithromycin", Category = MedicineCategory.Antibiotic, DefaultDosage = "250mg",  IsActive = true },
                new Medicine { MedicineId = 8, Name = "Azithromycin 500mg", GenericName = "Azithromycin", Category = MedicineCategory.Antibiotic, DefaultDosage = "500mg",  IsActive = true },
                new Medicine { MedicineId = 9, Name = "Ciprofloxacin 500mg", GenericName = "Ciprofloxacin", Category = MedicineCategory.Antibiotic, DefaultDosage = "500mg", IsActive = true },
                new Medicine { MedicineId = 10, Name = "Doxycycline 100mg", GenericName = "Doxycycline", Category = MedicineCategory.Antibiotic, DefaultDosage = "100mg",  IsActive = true },

                // Antacids / GI
                new Medicine { MedicineId = 11, Name = "Omeprazole 20mg", GenericName = "Omeprazole", Category = MedicineCategory.Antacid, DefaultDosage = "20mg", 
                    IsActive = true },
                new Medicine { MedicineId = 12, Name = "Pantoprazole 40mg", GenericName = "Pantoprazole", Category = MedicineCategory.Antacid, DefaultDosage = "40mg",  IsActive = true },
                new Medicine { MedicineId = 13, Name = "Ranitidine 150mg", GenericName = "Ranitidine", Category = MedicineCategory.Antacid, DefaultDosage = "150mg",  IsActive = true },
                new Medicine { MedicineId = 14, Name = "Domperidone 10mg", GenericName = "Domperidone", Category = MedicineCategory.Antacid, DefaultDosage = "10mg",  IsActive = true },

                // Antihistamines
                new Medicine { MedicineId = 15, Name = "Cetirizine 10mg", GenericName = "Cetirizine", Category = MedicineCategory.Antihistamine, DefaultDosage = "10mg",  IsActive = true },
                new Medicine { MedicineId = 16, Name = "Loratadine 10mg", GenericName = "Loratadine", Category = MedicineCategory.Antihistamine, DefaultDosage = "10mg",  IsActive = true },
                new Medicine { MedicineId = 17, Name = "Chlorpheniramine 4mg", GenericName = "Chlorpheniramine", Category = MedicineCategory.Antihistamine, DefaultDosage = "4mg",  IsActive = true },

                // Antidiabetics
                new Medicine { MedicineId = 18, Name = "Metformin 500mg", GenericName = "Metformin", Category = MedicineCategory.Antidiabetic, DefaultDosage = "500mg",  IsActive = true },
                new Medicine { MedicineId = 19, Name = "Metformin 1000mg", GenericName = "Metformin", Category = MedicineCategory.Antidiabetic, DefaultDosage = "1000mg",  IsActive = true },
                new Medicine { MedicineId = 20, Name = "Glibenclamide 5mg", GenericName = "Glibenclamide", Category = MedicineCategory.Antidiabetic, DefaultDosage = "5mg",  IsActive = true },
                new Medicine { MedicineId = 21, Name = "Glimepiride 2mg", GenericName = "Glimepiride", Category = MedicineCategory.Antidiabetic, DefaultDosage = "2mg", IsActive = true },

                // Antihypertensives
                new Medicine { MedicineId = 22, Name = "Amlodipine 5mg", GenericName = "Amlodipine", Category = MedicineCategory.Antihypertensive, DefaultDosage = "5mg",  IsActive = true },
                new Medicine { MedicineId = 23, Name = "Amlodipine 10mg", GenericName = "Amlodipine", Category = MedicineCategory.Antihypertensive, DefaultDosage = "10mg",  IsActive = true },
                new Medicine { MedicineId = 24, Name = "Losartan 50mg", GenericName = "Losartan", Category = MedicineCategory.Antihypertensive, DefaultDosage = "50mg",  IsActive = true },
                new Medicine { MedicineId = 25, Name = "Atenolol 50mg", GenericName = "Atenolol", Category = MedicineCategory.Antihypertensive, DefaultDosage = "50mg",  IsActive = true },

                // Vitamins / Supplements
                new Medicine { MedicineId = 26, Name = "Vitamin C 500mg", GenericName = "Ascorbic Acid", Category = MedicineCategory.Vitamin, DefaultDosage = "500mg",  IsActive = true },
                new Medicine { MedicineId = 27, Name = "Vitamin D3 60000 IU", GenericName = "Cholecalciferol", Category = MedicineCategory.Vitamin, DefaultDosage = "60000IU",  IsActive = true },
                new Medicine { MedicineId = 28, Name = "Vitamin B12 500mcg", GenericName = "Cyanocobalamin", Category = MedicineCategory.Vitamin, DefaultDosage = "500mcg",  IsActive = true },
                new Medicine { MedicineId = 29, Name = "Calcium + Vitamin D3", GenericName = "Calcium Carbonate", Category = MedicineCategory.Supplement, DefaultDosage = "500mg",  IsActive = true },
                new Medicine { MedicineId = 30, Name = "Iron + Folic Acid", GenericName = "Ferrous Sulfate", Category = MedicineCategory.Supplement, DefaultDosage = "150mg",  IsActive = true },

                // Antifungals
                new Medicine { MedicineId = 31, Name = "Fluconazole 150mg", GenericName = "Fluconazole", Category = MedicineCategory.Antifungal, DefaultDosage = "150mg",  IsActive = true },
                new Medicine { MedicineId = 32, Name = "Clotrimazole 1% Cream", GenericName = "Clotrimazole", Category = MedicineCategory.Antifungal, DefaultDosage = "1%",  IsActive = true },

                // Antivirals
                new Medicine { MedicineId = 33, Name = "Acyclovir 400mg", GenericName = "Acyclovir", Category = MedicineCategory.Antiviral, DefaultDosage = "400mg",  IsActive = true },
                new Medicine { MedicineId = 34, Name = "Oseltamivir 75mg", GenericName = "Oseltamivir", Category = MedicineCategory.Antiviral, DefaultDosage = "75mg", IsActive = true },

                // Steroids
                new Medicine { MedicineId = 35, Name = "Prednisolone 5mg", GenericName = "Prednisolone", Category = MedicineCategory.Steroid, DefaultDosage = "5mg", IsActive = true },
                new Medicine { MedicineId = 36, Name = "Dexamethasone 0.5mg", GenericName = "Dexamethasone", Category = MedicineCategory.Steroid, DefaultDosage = "0.5mg", IsActive = true },

                // Bronchodilators
                new Medicine { MedicineId = 37, Name = "Salbutamol 2mg", GenericName = "Salbutamol", Category = MedicineCategory.Bronchodilator, DefaultDosage = "2mg",  IsActive = true },
                new Medicine { MedicineId = 38, Name = "Montelukast 10mg", GenericName = "Montelukast", Category = MedicineCategory.Bronchodilator, DefaultDosage = "10mg", IsActive = true },

                // Diuretics
                new Medicine { MedicineId = 39, Name = "Furosemide 40mg", GenericName = "Furosemide", Category = MedicineCategory.Diuretic, DefaultDosage = "40mg",  IsActive = true },
                new Medicine { MedicineId = 40, Name = "Spironolactone 25mg", GenericName = "Spironolactone", Category = MedicineCategory.Diuretic, DefaultDosage = "25mg",IsActive = true }
            );
        }

        // ─── Lab Test Catalog ─────────────────────────────────────────────────────
        private static void SeedLabTestCatalog(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LabTestCatalog>().HasData(

                // Blood Tests
                new LabTestCatalog { TestCatalogId = 1, TestName = "Complete Blood Count", ShortName = "CBC", Category = TestCategory.BloodTest, NormalRange = "RBC: 4.5-5.5M/µL, WBC: 4000-11000/µL, Hb: 13-17 g/dL",  IsActive = true },
                new LabTestCatalog { TestCatalogId = 2, TestName = "Liver Function Test", ShortName = "LFT", Category = TestCategory.BloodTest, NormalRange = "ALT: 7-56 U/L, AST: 10-40 U/L, ALP: 44-147 U/L",  IsActive = true },
                new LabTestCatalog { TestCatalogId = 3, TestName = "Kidney Function Test", ShortName = "KFT", Category = TestCategory.BloodTest, NormalRange = "Creatinine: 0.6-1.2 mg/dL, Urea: 15-40 mg/dL",  IsActive = true },
                new LabTestCatalog { TestCatalogId = 4, TestName = "Fasting Blood Sugar", ShortName = "FBS", Category = TestCategory.BloodTest, NormalRange = "70-100 mg/dL", IsActive = true },
                new LabTestCatalog { TestCatalogId = 5, TestName = "Postprandial Blood Sugar", ShortName = "PPBS", Category = TestCategory.BloodTest, NormalRange = "< 140 mg/dL",  IsActive = true },
                new LabTestCatalog { TestCatalogId = 6, TestName = "HbA1c", ShortName = "HbA1c", Category = TestCategory.BloodTest, NormalRange = "< 5.7% (Normal), 5.7-6.4% (Prediabetic)",  IsActive = true },
                new LabTestCatalog { TestCatalogId = 7, TestName = "Lipid Profile", ShortName = "LP", Category = TestCategory.BloodTest, NormalRange = "Total Cholesterol < 200 mg/dL, LDL < 100 mg/dL", IsActive = true },
                new LabTestCatalog { TestCatalogId = 8, TestName = "Serum Electrolytes", ShortName = "ELEC", Category = TestCategory.BloodTest, NormalRange = "Na: 135-145 mEq/L, K: 3.5-5.0 mEq/L",  IsActive = true },
                new LabTestCatalog { TestCatalogId = 9, TestName = "C-Reactive Protein", ShortName = "CRP", Category = TestCategory.BloodTest, NormalRange = "< 10 mg/L",  IsActive = true },
                new LabTestCatalog { TestCatalogId = 10, TestName = "Erythrocyte Sedimentation Rate", ShortName = "ESR", Category = TestCategory.BloodTest, NormalRange = "Men: < 15 mm/hr, Women: < 20 mm/hr",  IsActive = true },
                new LabTestCatalog { TestCatalogId = 11, TestName = "Prothrombin Time", ShortName = "PT/INR", Category = TestCategory.BloodTest, NormalRange = "INR: 0.8-1.1",  IsActive = true },
                new LabTestCatalog { TestCatalogId = 12, TestName = "Blood Group & Rh Typing", ShortName = "BG", Category = TestCategory.BloodTest, NormalRange = null,  IsActive = true },

                // Hormone Tests
                new LabTestCatalog { TestCatalogId = 13, TestName = "Thyroid Stimulating Hormone", ShortName = "TSH", Category = TestCategory.HormoneTest, NormalRange = "0.4-4.0 mIU/L",  IsActive = true },
                new LabTestCatalog { TestCatalogId = 14, TestName = "Free T3", ShortName = "FT3", Category = TestCategory.HormoneTest, NormalRange = "2.3-4.2 pg/mL",  IsActive = true },
                new LabTestCatalog { TestCatalogId = 15, TestName = "Free T4", ShortName = "FT4", Category = TestCategory.HormoneTest, NormalRange = "0.8-1.8 ng/dL", IsActive = true },
                new LabTestCatalog { TestCatalogId = 16, TestName = "Thyroid Profile (T3, T4, TSH)", ShortName = "TFT", Category = TestCategory.HormoneTest, NormalRange = "TSH: 0.4-4.0 mIU/L", IsActive = true },
                new LabTestCatalog { TestCatalogId = 17, TestName = "Testosterone", ShortName = "TESTO", Category = TestCategory.HormoneTest, NormalRange = "Men: 270-1070 ng/dL",  IsActive = true },
                new LabTestCatalog { TestCatalogId = 18, TestName = "Prolactin", ShortName = "PRL", Category = TestCategory.HormoneTest, NormalRange = "Men: 2-18 ng/mL, Women: 2-29 ng/mL",  IsActive = true },

                // Urine Tests
                new LabTestCatalog { TestCatalogId = 19, TestName = "Urine Routine Examination", ShortName = "URE", Category = TestCategory.UrineTest, NormalRange = "pH: 4.5-8.0, Protein: Negative, Glucose: Negative",  IsActive = true },
                new LabTestCatalog { TestCatalogId = 20, TestName = "Urine Culture & Sensitivity", ShortName = "UCS", Category = TestCategory.UrineTest, NormalRange = "No growth", IsActive = true },
                new LabTestCatalog { TestCatalogId = 21, TestName = "24-Hour Urine Protein", ShortName = "24UPR", Category = TestCategory.UrineTest, NormalRange = "< 150 mg/day",  IsActive = true },

                // Microbiology Tests
                new LabTestCatalog { TestCatalogId = 22, TestName = "Dengue NS1 Antigen", ShortName = "NS1", Category = TestCategory.MicrobiologyTest, NormalRange = "Negative",  IsActive = true },
                new LabTestCatalog { TestCatalogId = 23, TestName = "Dengue IgM / IgG", ShortName = "DENG-AB", Category = TestCategory.MicrobiologyTest, NormalRange = "Negative", IsActive = true },
                new LabTestCatalog { TestCatalogId = 24, TestName = "Malaria Antigen Test", ShortName = "MAL", Category = TestCategory.MicrobiologyTest, NormalRange = "Negative",  IsActive = true },
                new LabTestCatalog { TestCatalogId = 25, TestName = "Typhoid (Widal Test)", ShortName = "WIDAL", Category = TestCategory.MicrobiologyTest, NormalRange = "Titer < 1:80", IsActive = true },
                new LabTestCatalog { TestCatalogId = 26, TestName = "COVID-19 RT-PCR", ShortName = "RTPCR", Category = TestCategory.MicrobiologyTest, NormalRange = "Not Detected",  IsActive = true },
                new LabTestCatalog { TestCatalogId = 27, TestName = "COVID-19 Rapid Antigen", ShortName = "RAT", Category = TestCategory.MicrobiologyTest, NormalRange = "Negative", IsActive = true },
                new LabTestCatalog { TestCatalogId = 28, TestName = "HIV 1 & 2 Antibody", ShortName = "HIV", Category = TestCategory.MicrobiologyTest, NormalRange = "Non-Reactive", IsActive = true },
                new LabTestCatalog { TestCatalogId = 29, TestName = "Hepatitis B Surface Antigen", ShortName = "HBsAg", Category = TestCategory.MicrobiologyTest, NormalRange = "Negative",  IsActive = true },
                new LabTestCatalog { TestCatalogId = 30, TestName = "Hepatitis C Antibody", ShortName = "HCV", Category = TestCategory.MicrobiologyTest, NormalRange = "Non-Reactive",  IsActive = true },

                // Cardiology Tests
                new LabTestCatalog { TestCatalogId = 31, TestName = "Electrocardiogram", ShortName = "ECG", Category = TestCategory.CardiologyTest, NormalRange = "Normal sinus rhythm",  IsActive = true },
                new LabTestCatalog { TestCatalogId = 32, TestName = "Echocardiogram", ShortName = "ECHO", Category = TestCategory.CardiologyTest, NormalRange = "Normal cardiac function",  IsActive = true },
                new LabTestCatalog { TestCatalogId = 33, TestName = "Troponin I", ShortName = "TROP-I", Category = TestCategory.CardiologyTest, NormalRange = "< 0.04 ng/mL",  IsActive = true },

                // Imaging
                new LabTestCatalog { TestCatalogId = 34, TestName = "Chest X-Ray", ShortName = "CXR", Category = TestCategory.Imaging, NormalRange = null,  IsActive = true },
                new LabTestCatalog { TestCatalogId = 35, TestName = "X-Ray (Other)", ShortName = "XR", Category = TestCategory.Imaging, NormalRange = null,  IsActive = true },
                new LabTestCatalog { TestCatalogId = 36, TestName = "Ultrasound Abdomen", ShortName = "USG-ABD", Category = TestCategory.Imaging, NormalRange = null,  IsActive = true },
                new LabTestCatalog { TestCatalogId = 37, TestName = "Ultrasound Pelvis", ShortName = "USG-PEL", Category = TestCategory.Imaging, NormalRange = null, IsActive = true },
                new LabTestCatalog { TestCatalogId = 38, TestName = "CT Scan Head", ShortName = "CT-HEAD", Category = TestCategory.Imaging, NormalRange = null,  IsActive = true },
                new LabTestCatalog { TestCatalogId = 39, TestName = "CT Scan Abdomen", ShortName = "CT-ABD", Category = TestCategory.Imaging, NormalRange = null, IsActive = true },
                new LabTestCatalog { TestCatalogId = 40, TestName = "MRI Brain", ShortName = "MRI-B", Category = TestCategory.Imaging, NormalRange = null,  IsActive = true },
                new LabTestCatalog { TestCatalogId = 41, TestName = "MRI Spine", ShortName = "MRI-SP", Category = TestCategory.Imaging, NormalRange = null,  IsActive = true },

                // Biopsy
                new LabTestCatalog { TestCatalogId = 42, TestName = "Fine Needle Aspiration Cytology", ShortName = "FNAC", Category = TestCategory.Biopsy, NormalRange = null,  IsActive = true },
                new LabTestCatalog { TestCatalogId = 43, TestName = "Histopathology", ShortName = "HPE", Category = TestCategory.Biopsy, NormalRange = null, IsActive = true }
            );
        }
    }
}