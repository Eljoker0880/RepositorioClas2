-- Script de creación y datos base para AdminGym
-- Tablas: Miembro, Membresia
-- Claves primarias y foráneas incluidas

PRAGMA foreign_keys = ON;

CREATE TABLE IF NOT EXISTS Miembro (
	id INTEGER PRIMARY KEY,
	Nombre TEXT NOT NULL,
	Apellido TEXT NOT NULL,
	Telefono TEXT NOT NULL,
	Fecha TEXT NOT NULL -- guardar fecha en formato ISO (YYYY-MM-DD)
);

CREATE TABLE IF NOT EXISTS Membresia (
	id INTEGER PRIMARY KEY,
	Tipo TEXT NOT NULL,
	Inscripcion TEXT NOT NULL, -- fecha ISO
	Vencimiento TEXT NOT NULL, -- fecha ISO
	id_miembro INTEGER NOT NULL,
	FOREIGN KEY (id_miembro) REFERENCES Miembro(id) ON DELETE CASCADE
);

-- Datos de ejemplo (coinciden con los valores usados en los servicios en memoria)
INSERT INTO Miembro (id, Nombre, Apellido, Telefono, Fecha) VALUES
(1, 'Demo', 'Demo', '123456789', '1990-01-01');

INSERT INTO Membresia (id, Tipo, Inscripcion, Vencimiento, id_miembro) VALUES
(1, '1 Mes', '2026-07-01', '2026-08-01', 1);

-- Consultas de ejemplo usando INNER JOIN
-- 1) Selección simple con INNER JOIN (mostrar Td, Nombre, Apellido, Teléfono, Tipo, Estado)
SELECT
	m.id AS Td,
	m.Nombre,
	m.Apellido,
	m.Telefono,
	COALESCE(b.Tipo, '-') AS Tipo,
	CASE
		WHEN b.Vencimiento IS NULL THEN 'Sin membresía'
		WHEN datetime('now') > datetime(b.Vencimiento) THEN 'Vencida'
		WHEN (julianday(b.Vencimiento) - julianday('now')) <= 15 THEN 'Próxima a vencer'
		ELSE 'Activo'
	END AS Estado
FROM Miembro m
LEFT JOIN Membresia b ON b.id_miembro = m.id;

-- 2) Ejemplo: listar sólo miembros activos
SELECT m.id, m.Nombre, m.Apellido, b.Tipo, b.Vencimiento
FROM Miembro m
INNER JOIN Membresia b ON b.id_miembro = m.id
WHERE datetime(b.Vencimiento) > datetime('now');

-- 3) Ejemplo: contar miembros por tipo de membresía
SELECT b.Tipo, COUNT(*) AS Cantidad
FROM Membresia b
GROUP BY b.Tipo;

-- Fin del script
