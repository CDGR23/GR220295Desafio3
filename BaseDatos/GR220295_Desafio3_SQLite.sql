-- Script de referencia para SQLite.
-- La aplicación utiliza EF Core Migrations como mecanismo principal de creación.

PRAGMA foreign_keys = ON;

CREATE TABLE IF NOT EXISTS AspNetRoles (
    Id TEXT NOT NULL CONSTRAINT PK_AspNetRoles PRIMARY KEY,
    Name TEXT NULL,
    NormalizedName TEXT NULL,
    ConcurrencyStamp TEXT NULL
);
CREATE UNIQUE INDEX IF NOT EXISTS RoleNameIndex ON AspNetRoles(NormalizedName);

CREATE TABLE IF NOT EXISTS AspNetUsers (
    Id TEXT NOT NULL CONSTRAINT PK_AspNetUsers PRIMARY KEY,
    UserName TEXT NULL,
    NormalizedUserName TEXT NULL,
    Email TEXT NULL,
    NormalizedEmail TEXT NULL,
    EmailConfirmed INTEGER NOT NULL,
    PasswordHash TEXT NULL,
    SecurityStamp TEXT NULL,
    ConcurrencyStamp TEXT NULL,
    PhoneNumber TEXT NULL,
    PhoneNumberConfirmed INTEGER NOT NULL,
    TwoFactorEnabled INTEGER NOT NULL,
    LockoutEnd TEXT NULL,
    LockoutEnabled INTEGER NOT NULL,
    AccessFailedCount INTEGER NOT NULL,
    NombreCompleto TEXT NOT NULL,
    Rol TEXT NOT NULL
);
CREATE UNIQUE INDEX IF NOT EXISTS UserNameIndex ON AspNetUsers(NormalizedUserName);
CREATE INDEX IF NOT EXISTS EmailIndex ON AspNetUsers(NormalizedEmail);

CREATE TABLE IF NOT EXISTS AspNetRoleClaims (
    Id INTEGER NOT NULL CONSTRAINT PK_AspNetRoleClaims PRIMARY KEY AUTOINCREMENT,
    RoleId TEXT NOT NULL, ClaimType TEXT NULL, ClaimValue TEXT NULL,
    CONSTRAINT FK_AspNetRoleClaims_AspNetRoles_RoleId FOREIGN KEY(RoleId) REFERENCES AspNetRoles(Id) ON DELETE CASCADE
);
CREATE INDEX IF NOT EXISTS IX_AspNetRoleClaims_RoleId ON AspNetRoleClaims(RoleId);

CREATE TABLE IF NOT EXISTS AspNetUserClaims (
    Id INTEGER NOT NULL CONSTRAINT PK_AspNetUserClaims PRIMARY KEY AUTOINCREMENT,
    UserId TEXT NOT NULL, ClaimType TEXT NULL, ClaimValue TEXT NULL,
    CONSTRAINT FK_AspNetUserClaims_AspNetUsers_UserId FOREIGN KEY(UserId) REFERENCES AspNetUsers(Id) ON DELETE CASCADE
);
CREATE INDEX IF NOT EXISTS IX_AspNetUserClaims_UserId ON AspNetUserClaims(UserId);

CREATE TABLE IF NOT EXISTS AspNetUserLogins (
    LoginProvider TEXT NOT NULL, ProviderKey TEXT NOT NULL, ProviderDisplayName TEXT NULL, UserId TEXT NOT NULL,
    CONSTRAINT PK_AspNetUserLogins PRIMARY KEY(LoginProvider, ProviderKey),
    CONSTRAINT FK_AspNetUserLogins_AspNetUsers_UserId FOREIGN KEY(UserId) REFERENCES AspNetUsers(Id) ON DELETE CASCADE
);
CREATE INDEX IF NOT EXISTS IX_AspNetUserLogins_UserId ON AspNetUserLogins(UserId);

CREATE TABLE IF NOT EXISTS AspNetUserRoles (
    UserId TEXT NOT NULL, RoleId TEXT NOT NULL,
    CONSTRAINT PK_AspNetUserRoles PRIMARY KEY(UserId, RoleId),
    CONSTRAINT FK_AspNetUserRoles_AspNetRoles_RoleId FOREIGN KEY(RoleId) REFERENCES AspNetRoles(Id) ON DELETE CASCADE,
    CONSTRAINT FK_AspNetUserRoles_AspNetUsers_UserId FOREIGN KEY(UserId) REFERENCES AspNetUsers(Id) ON DELETE CASCADE
);
CREATE INDEX IF NOT EXISTS IX_AspNetUserRoles_RoleId ON AspNetUserRoles(RoleId);

CREATE TABLE IF NOT EXISTS AspNetUserTokens (
    UserId TEXT NOT NULL, LoginProvider TEXT NOT NULL, Name TEXT NOT NULL, Value TEXT NULL,
    CONSTRAINT PK_AspNetUserTokens PRIMARY KEY(UserId, LoginProvider, Name),
    CONSTRAINT FK_AspNetUserTokens_AspNetUsers_UserId FOREIGN KEY(UserId) REFERENCES AspNetUsers(Id) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS Plazas (
    IdPlaza INTEGER NOT NULL CONSTRAINT PK_Plazas PRIMARY KEY AUTOINCREMENT,
    Titulo TEXT NOT NULL,
    Descripcion TEXT NOT NULL,
    FechaPublicacion TEXT NOT NULL,
    FechaCierre TEXT NOT NULL,
    Institucion TEXT NOT NULL,
    Estado TEXT NOT NULL,
    IdAgente TEXT NOT NULL,
    CONSTRAINT FK_Plazas_AspNetUsers_IdAgente FOREIGN KEY(IdAgente) REFERENCES AspNetUsers(Id) ON DELETE RESTRICT
);
CREATE INDEX IF NOT EXISTS IX_Plazas_IdAgente ON Plazas(IdAgente);

CREATE TABLE IF NOT EXISTS Postulaciones (
    IdPostulacion INTEGER NOT NULL CONSTRAINT PK_Postulaciones PRIMARY KEY AUTOINCREMENT,
    IdPlaza INTEGER NOT NULL,
    IdCandidato TEXT NOT NULL,
    FechaPostulacion TEXT NOT NULL,
    Estado TEXT NOT NULL,
    CONSTRAINT FK_Postulaciones_AspNetUsers_IdCandidato FOREIGN KEY(IdCandidato) REFERENCES AspNetUsers(Id) ON DELETE RESTRICT,
    CONSTRAINT FK_Postulaciones_Plazas_IdPlaza FOREIGN KEY(IdPlaza) REFERENCES Plazas(IdPlaza) ON DELETE CASCADE
);
CREATE UNIQUE INDEX IF NOT EXISTS IX_Postulaciones_IdPlaza_IdCandidato ON Postulaciones(IdPlaza, IdCandidato);

CREATE TABLE IF NOT EXISTS HojasDeVida (
    IdCV INTEGER NOT NULL CONSTRAINT PK_HojasDeVida PRIMARY KEY AUTOINCREMENT,
    IdUsuario TEXT NOT NULL,
    FormacionAcademica TEXT NOT NULL,
    ExperienciaLaboral TEXT NOT NULL,
    Competencias TEXT NOT NULL,
    ArchivoCV TEXT NOT NULL,
    CONSTRAINT FK_HojasDeVida_AspNetUsers_IdUsuario FOREIGN KEY(IdUsuario) REFERENCES AspNetUsers(Id) ON DELETE CASCADE
);
CREATE UNIQUE INDEX IF NOT EXISTS IX_HojasDeVida_IdUsuario ON HojasDeVida(IdUsuario);

CREATE TABLE IF NOT EXISTS __EFMigrationsHistory (
    MigrationId TEXT NOT NULL CONSTRAINT PK___EFMigrationsHistory PRIMARY KEY,
    ProductVersion TEXT NOT NULL
);
