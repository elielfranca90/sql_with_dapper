CREATE TABLE IF NOT EXISTS produto (
    id SERIAL PRIMARY KEY,
    codigo VARCHAR(50) NOT NULL,
    descricao VARCHAR(255) NOT NULL
);

-- Inserir dados iniciais para teste
-- Produtos relacionados a café

INSERT INTO produto (codigo, descricao) VALUES
('CAF001', 'Café em grãos arábica 1kg'),
('CAF002', 'Café em grãos robusta 1kg'),
('CAF003', 'Café moído tradicional 500g'),
('CAF004', 'Café moído gourmet 500g'),
('CAF005', 'Café em cápsulas compatível Nespresso'),
('CAF006', 'Café em cápsulas compatível Dolce Gusto'),
('CAF007', 'Máquina de café expresso doméstica'),
('CAF008', 'Máquina de café expresso profissional'),
('CAF009', 'Moedor de café manual'),
('CAF010', 'Moedor de café elétrico'),
('CAF011', 'Filtro de café de papel nº 103'),
('CAF012', 'Filtro de café permanente reutilizável'),
('CAF013', 'Prensa francesa 600ml'),
('CAF014', 'Cafeteira italiana (Moka) 6 xícaras'),
('CAF015', 'Chaleira elétrica com controle de temperatura'),
('CAF016', 'Balança digital para café'),
('CAF017', 'Tamper inox para café expresso'),
('CAF018', 'Porta-filtro profissional 58mm'),
('CAF019', 'Xícara para café expresso 60ml'),
('CAF020', 'Kit barista iniciante');
