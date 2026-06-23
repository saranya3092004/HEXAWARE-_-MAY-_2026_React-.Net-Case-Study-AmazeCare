using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AmazeCare.Server.Migrations
{
    /// <inheritdoc />
    public partial class initialmigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LabTestCatalogs",
                columns: table => new
                {
                    TestCatalogId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TestName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ShortName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NormalRange = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LabTestCatalogs", x => x.TestCatalogId);
                });

            migrationBuilder.CreateTable(
                name: "Medicines",
                columns: table => new
                {
                    MedicineId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    GenericName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DefaultDosage = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Medicines", x => x.MedicineId);
                });

            migrationBuilder.CreateTable(
                name: "Specializations",
                columns: table => new
                {
                    SpecializationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Specializations", x => x.SpecializationId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    Role = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Admins",
                columns: table => new
                {
                    AdminId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admins", x => x.AdminId);
                    table.ForeignKey(
                        name: "FK_Admins_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Doctors",
                columns: table => new
                {
                    DoctorId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SpecializationId = table.Column<int>(type: "int", nullable: false),
                    Qualification = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Designation = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ExperienceYears = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Doctors", x => x.DoctorId);
                    table.ForeignKey(
                        name: "FK_Doctors_Specializations_SpecializationId",
                        column: x => x.SpecializationId,
                        principalTable: "Specializations",
                        principalColumn: "SpecializationId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Doctors_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Patients",
                columns: table => new
                {
                    PatientId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "date", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patients", x => x.PatientId);
                    table.ForeignKey(
                        name: "FK_Patients_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Appointments",
                columns: table => new
                {
                    AppointmentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    DoctorId = table.Column<int>(type: "int", nullable: false),
                    AppointmentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TimeSlot = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    VisitType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false, defaultValue: "GeneralCheckup"),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Pending"),
                    CancellationReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    BookedByUserUserId = table.Column<int>(type: "int", nullable: false),
                    SpecializationId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Appointments", x => x.AppointmentId);
                    table.ForeignKey(
                        name: "FK_Appointments_Doctors_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "Doctors",
                        principalColumn: "DoctorId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Appointments_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "PatientId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Appointments_Specializations_SpecializationId",
                        column: x => x.SpecializationId,
                        principalTable: "Specializations",
                        principalColumn: "SpecializationId");
                    table.ForeignKey(
                        name: "FK_Appointments_Users_BookedByUserUserId",
                        column: x => x.BookedByUserUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Consultations",
                columns: table => new
                {
                    ConsultationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AppointmentId = table.Column<int>(type: "int", nullable: false),
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    DoctorId = table.Column<int>(type: "int", nullable: false),
                    CurrentSymptoms = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    PhysicalExamination = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    TreatmentPlan = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Diagnosis = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ConsultationDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Consultations", x => x.ConsultationId);
                    table.ForeignKey(
                        name: "FK_Consultations_Appointments_AppointmentId",
                        column: x => x.AppointmentId,
                        principalTable: "Appointments",
                        principalColumn: "AppointmentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Consultations_Doctors_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "Doctors",
                        principalColumn: "DoctorId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Consultations_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "PatientId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LabTests",
                columns: table => new
                {
                    LabTestId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ConsultationId = table.Column<int>(type: "int", nullable: false),
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    DoctorId = table.Column<int>(type: "int", nullable: false),
                    TestCatalogId = table.Column<int>(type: "int", nullable: false),
                    OrderedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LabTests", x => x.LabTestId);
                    table.ForeignKey(
                        name: "FK_LabTests_Consultations_ConsultationId",
                        column: x => x.ConsultationId,
                        principalTable: "Consultations",
                        principalColumn: "ConsultationId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LabTests_Doctors_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "Doctors",
                        principalColumn: "DoctorId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LabTests_LabTestCatalogs_TestCatalogId",
                        column: x => x.TestCatalogId,
                        principalTable: "LabTestCatalogs",
                        principalColumn: "TestCatalogId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LabTests_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "PatientId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Prescriptions",
                columns: table => new
                {
                    PrescriptionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ConsultationId = table.Column<int>(type: "int", nullable: false),
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    DoctorId = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    PrescribedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prescriptions", x => x.PrescriptionId);
                    table.ForeignKey(
                        name: "FK_Prescriptions_Consultations_ConsultationId",
                        column: x => x.ConsultationId,
                        principalTable: "Consultations",
                        principalColumn: "ConsultationId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Prescriptions_Doctors_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "Doctors",
                        principalColumn: "DoctorId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Prescriptions_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "PatientId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PrescriptionItems",
                columns: table => new
                {
                    PrescriptionItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PrescriptionId = table.Column<int>(type: "int", nullable: false),
                    MedicineId = table.Column<int>(type: "int", nullable: false),
                    Morning = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Afternoon = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Evening = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Night = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    FoodInstruction = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "AfterFood"),
                    Dosage = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DurationDays = table.Column<int>(type: "int", nullable: false),
                    Instructions = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrescriptionItems", x => x.PrescriptionItemId);
                    table.ForeignKey(
                        name: "FK_PrescriptionItems_Medicines_MedicineId",
                        column: x => x.MedicineId,
                        principalTable: "Medicines",
                        principalColumn: "MedicineId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PrescriptionItems_Prescriptions_PrescriptionId",
                        column: x => x.PrescriptionId,
                        principalTable: "Prescriptions",
                        principalColumn: "PrescriptionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "LabTestCatalogs",
                columns: new[] { "TestCatalogId", "Category", "IsActive", "NormalRange", "ShortName", "TestName" },
                values: new object[,]
                {
                    { 1, "BloodTest", true, "RBC: 4.5-5.5M/µL, WBC: 4000-11000/µL, Hb: 13-17 g/dL", "CBC", "Complete Blood Count" },
                    { 2, "BloodTest", true, "ALT: 7-56 U/L, AST: 10-40 U/L, ALP: 44-147 U/L", "LFT", "Liver Function Test" },
                    { 3, "BloodTest", true, "Creatinine: 0.6-1.2 mg/dL, Urea: 15-40 mg/dL", "KFT", "Kidney Function Test" },
                    { 4, "BloodTest", true, "70-100 mg/dL", "FBS", "Fasting Blood Sugar" },
                    { 5, "BloodTest", true, "< 140 mg/dL", "PPBS", "Postprandial Blood Sugar" },
                    { 6, "BloodTest", true, "< 5.7% (Normal), 5.7-6.4% (Prediabetic)", "HbA1c", "HbA1c" },
                    { 7, "BloodTest", true, "Total Cholesterol < 200 mg/dL, LDL < 100 mg/dL", "LP", "Lipid Profile" },
                    { 8, "BloodTest", true, "Na: 135-145 mEq/L, K: 3.5-5.0 mEq/L", "ELEC", "Serum Electrolytes" },
                    { 9, "BloodTest", true, "< 10 mg/L", "CRP", "C-Reactive Protein" },
                    { 10, "BloodTest", true, "Men: < 15 mm/hr, Women: < 20 mm/hr", "ESR", "Erythrocyte Sedimentation Rate" },
                    { 11, "BloodTest", true, "INR: 0.8-1.1", "PT/INR", "Prothrombin Time" },
                    { 12, "BloodTest", true, null, "BG", "Blood Group & Rh Typing" },
                    { 13, "HormoneTest", true, "0.4-4.0 mIU/L", "TSH", "Thyroid Stimulating Hormone" },
                    { 14, "HormoneTest", true, "2.3-4.2 pg/mL", "FT3", "Free T3" },
                    { 15, "HormoneTest", true, "0.8-1.8 ng/dL", "FT4", "Free T4" },
                    { 16, "HormoneTest", true, "TSH: 0.4-4.0 mIU/L", "TFT", "Thyroid Profile (T3, T4, TSH)" },
                    { 17, "HormoneTest", true, "Men: 270-1070 ng/dL", "TESTO", "Testosterone" },
                    { 18, "HormoneTest", true, "Men: 2-18 ng/mL, Women: 2-29 ng/mL", "PRL", "Prolactin" },
                    { 19, "UrineTest", true, "pH: 4.5-8.0, Protein: Negative, Glucose: Negative", "URE", "Urine Routine Examination" },
                    { 20, "UrineTest", true, "No growth", "UCS", "Urine Culture & Sensitivity" },
                    { 21, "UrineTest", true, "< 150 mg/day", "24UPR", "24-Hour Urine Protein" },
                    { 22, "MicrobiologyTest", true, "Negative", "NS1", "Dengue NS1 Antigen" },
                    { 23, "MicrobiologyTest", true, "Negative", "DENG-AB", "Dengue IgM / IgG" },
                    { 24, "MicrobiologyTest", true, "Negative", "MAL", "Malaria Antigen Test" },
                    { 25, "MicrobiologyTest", true, "Titer < 1:80", "WIDAL", "Typhoid (Widal Test)" },
                    { 26, "MicrobiologyTest", true, "Not Detected", "RTPCR", "COVID-19 RT-PCR" },
                    { 27, "MicrobiologyTest", true, "Negative", "RAT", "COVID-19 Rapid Antigen" },
                    { 28, "MicrobiologyTest", true, "Non-Reactive", "HIV", "HIV 1 & 2 Antibody" },
                    { 29, "MicrobiologyTest", true, "Negative", "HBsAg", "Hepatitis B Surface Antigen" },
                    { 30, "MicrobiologyTest", true, "Non-Reactive", "HCV", "Hepatitis C Antibody" },
                    { 31, "CardiologyTest", true, "Normal sinus rhythm", "ECG", "Electrocardiogram" },
                    { 32, "CardiologyTest", true, "Normal cardiac function", "ECHO", "Echocardiogram" },
                    { 33, "CardiologyTest", true, "< 0.04 ng/mL", "TROP-I", "Troponin I" },
                    { 34, "Imaging", true, null, "CXR", "Chest X-Ray" },
                    { 35, "Imaging", true, null, "XR", "X-Ray (Other)" },
                    { 36, "Imaging", true, null, "USG-ABD", "Ultrasound Abdomen" },
                    { 37, "Imaging", true, null, "USG-PEL", "Ultrasound Pelvis" },
                    { 38, "Imaging", true, null, "CT-HEAD", "CT Scan Head" },
                    { 39, "Imaging", true, null, "CT-ABD", "CT Scan Abdomen" },
                    { 40, "Imaging", true, null, "MRI-B", "MRI Brain" },
                    { 41, "Imaging", true, null, "MRI-SP", "MRI Spine" },
                    { 42, "Biopsy", true, null, "FNAC", "Fine Needle Aspiration Cytology" },
                    { 43, "Biopsy", true, null, "HPE", "Histopathology" }
                });

            migrationBuilder.InsertData(
                table: "Medicines",
                columns: new[] { "MedicineId", "Category", "DefaultDosage", "GenericName", "IsActive", "Name" },
                values: new object[,]
                {
                    { 1, "Antipyretic", "500mg", "Acetaminophen", true, "Paracetamol 500mg" },
                    { 2, "Antipyretic", "650mg", "Acetaminophen", true, "Paracetamol 650mg" },
                    { 3, "Analgesic", "400mg", "Ibuprofen", true, "Ibuprofen 400mg" },
                    { 4, "Analgesic", "50mg", "Diclofenac Sodium", true, "Diclofenac 50mg" },
                    { 5, "Antibiotic", "250mg", "Amoxicillin", true, "Amoxicillin 250mg" },
                    { 6, "Antibiotic", "500mg", "Amoxicillin", true, "Amoxicillin 500mg" },
                    { 7, "Antibiotic", "250mg", "Azithromycin", true, "Azithromycin 250mg" },
                    { 8, "Antibiotic", "500mg", "Azithromycin", true, "Azithromycin 500mg" },
                    { 9, "Antibiotic", "500mg", "Ciprofloxacin", true, "Ciprofloxacin 500mg" },
                    { 10, "Antibiotic", "100mg", "Doxycycline", true, "Doxycycline 100mg" },
                    { 11, "Antacid", "20mg", "Omeprazole", true, "Omeprazole 20mg" },
                    { 12, "Antacid", "40mg", "Pantoprazole", true, "Pantoprazole 40mg" },
                    { 13, "Antacid", "150mg", "Ranitidine", true, "Ranitidine 150mg" },
                    { 14, "Antacid", "10mg", "Domperidone", true, "Domperidone 10mg" },
                    { 15, "Antihistamine", "10mg", "Cetirizine", true, "Cetirizine 10mg" },
                    { 16, "Antihistamine", "10mg", "Loratadine", true, "Loratadine 10mg" },
                    { 17, "Antihistamine", "4mg", "Chlorpheniramine", true, "Chlorpheniramine 4mg" },
                    { 18, "Antidiabetic", "500mg", "Metformin", true, "Metformin 500mg" },
                    { 19, "Antidiabetic", "1000mg", "Metformin", true, "Metformin 1000mg" },
                    { 20, "Antidiabetic", "5mg", "Glibenclamide", true, "Glibenclamide 5mg" },
                    { 21, "Antidiabetic", "2mg", "Glimepiride", true, "Glimepiride 2mg" },
                    { 22, "Antihypertensive", "5mg", "Amlodipine", true, "Amlodipine 5mg" },
                    { 23, "Antihypertensive", "10mg", "Amlodipine", true, "Amlodipine 10mg" },
                    { 24, "Antihypertensive", "50mg", "Losartan", true, "Losartan 50mg" },
                    { 25, "Antihypertensive", "50mg", "Atenolol", true, "Atenolol 50mg" },
                    { 26, "Vitamin", "500mg", "Ascorbic Acid", true, "Vitamin C 500mg" },
                    { 27, "Vitamin", "60000IU", "Cholecalciferol", true, "Vitamin D3 60000 IU" },
                    { 28, "Vitamin", "500mcg", "Cyanocobalamin", true, "Vitamin B12 500mcg" },
                    { 29, "Supplement", "500mg", "Calcium Carbonate", true, "Calcium + Vitamin D3" },
                    { 30, "Supplement", "150mg", "Ferrous Sulfate", true, "Iron + Folic Acid" },
                    { 31, "Antifungal", "150mg", "Fluconazole", true, "Fluconazole 150mg" },
                    { 32, "Antifungal", "1%", "Clotrimazole", true, "Clotrimazole 1% Cream" },
                    { 33, "Antiviral", "400mg", "Acyclovir", true, "Acyclovir 400mg" },
                    { 34, "Antiviral", "75mg", "Oseltamivir", true, "Oseltamivir 75mg" },
                    { 35, "Steroid", "5mg", "Prednisolone", true, "Prednisolone 5mg" },
                    { 36, "Steroid", "0.5mg", "Dexamethasone", true, "Dexamethasone 0.5mg" },
                    { 37, "Bronchodilator", "2mg", "Salbutamol", true, "Salbutamol 2mg" },
                    { 38, "Bronchodilator", "10mg", "Montelukast", true, "Montelukast 10mg" },
                    { 39, "Diuretic", "40mg", "Furosemide", true, "Furosemide 40mg" },
                    { 40, "Diuretic", "25mg", "Spironolactone", true, "Spironolactone 25mg" }
                });

            migrationBuilder.InsertData(
                table: "Specializations",
                columns: new[] { "SpecializationId", "IsActive", "Name" },
                values: new object[,]
                {
                    { 1, true, "General Medicine" },
                    { 2, true, "Obstetrics and Gynecology" },
                    { 3, true, "Pediatrics" },
                    { 4, true, "Cardiology" },
                    { 5, true, "Orthopedics" },
                    { 6, true, "Dermatology" },
                    { 7, true, "Neurology" },
                    { 8, true, "Ophthalmology" },
                    { 9, true, "ENT" },
                    { 10, true, "Psychiatry" },
                    { 11, true, "Urology" },
                    { 12, true, "Gastroenterology" },
                    { 13, true, "Pulmonology" },
                    { 14, true, "Endocrinology" },
                    { 15, true, "Oncology" },
                    { 16, true, "Nephrology" },
                    { 17, true, "Rheumatology" },
                    { 18, true, "Emergency Medicine" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Admins_UserId",
                table: "Admins",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_BookedByUserUserId",
                table: "Appointments",
                column: "BookedByUserUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_Doctor_Date",
                table: "Appointments",
                columns: new[] { "DoctorId", "AppointmentDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_Doctor_Date_TimeSlot_Unique",
                table: "Appointments",
                columns: new[] { "DoctorId", "AppointmentDate", "TimeSlot" },
                unique: true,
                filter: "[Status] <> 'Cancelled' AND [Status] <> 'Rejected' AND [Status] <> 'NoShow'");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_Patient_Date",
                table: "Appointments",
                columns: new[] { "PatientId", "AppointmentDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_SpecializationId",
                table: "Appointments",
                column: "SpecializationId");

            migrationBuilder.CreateIndex(
                name: "IX_Consultations_AppointmentId",
                table: "Consultations",
                column: "AppointmentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Consultations_DoctorId",
                table: "Consultations",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_Consultations_PatientId",
                table: "Consultations",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_Doctors_SpecializationId",
                table: "Doctors",
                column: "SpecializationId");

            migrationBuilder.CreateIndex(
                name: "IX_Doctors_UserId",
                table: "Doctors",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LabTestCatalogs_TestName",
                table: "LabTestCatalogs",
                column: "TestName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LabTests_ConsultationId",
                table: "LabTests",
                column: "ConsultationId");

            migrationBuilder.CreateIndex(
                name: "IX_LabTests_DoctorId",
                table: "LabTests",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_LabTests_PatientId",
                table: "LabTests",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_LabTests_TestCatalogId",
                table: "LabTests",
                column: "TestCatalogId");

            migrationBuilder.CreateIndex(
                name: "IX_Medicines_Name",
                table: "Medicines",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Patients_UserId",
                table: "Patients",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PrescriptionItems_MedicineId",
                table: "PrescriptionItems",
                column: "MedicineId");

            migrationBuilder.CreateIndex(
                name: "IX_PrescriptionItems_PrescriptionId",
                table: "PrescriptionItems",
                column: "PrescriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_Prescriptions_ConsultationId",
                table: "Prescriptions",
                column: "ConsultationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Prescriptions_DoctorId",
                table: "Prescriptions",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_Prescriptions_PatientId",
                table: "Prescriptions",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_Specializations_Name",
                table: "Specializations",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true,
                filter: "[Email] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Users_PhoneNumber",
                table: "Users",
                column: "PhoneNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Admins");

            migrationBuilder.DropTable(
                name: "LabTests");

            migrationBuilder.DropTable(
                name: "PrescriptionItems");

            migrationBuilder.DropTable(
                name: "LabTestCatalogs");

            migrationBuilder.DropTable(
                name: "Medicines");

            migrationBuilder.DropTable(
                name: "Prescriptions");

            migrationBuilder.DropTable(
                name: "Consultations");

            migrationBuilder.DropTable(
                name: "Appointments");

            migrationBuilder.DropTable(
                name: "Doctors");

            migrationBuilder.DropTable(
                name: "Patients");

            migrationBuilder.DropTable(
                name: "Specializations");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
