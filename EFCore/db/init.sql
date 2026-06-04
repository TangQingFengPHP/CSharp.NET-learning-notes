SET NAMES utf8mb4;
SET CHARACTER SET utf8mb4;

DROP DATABASE IF EXISTS efcore_demo;
CREATE DATABASE efcore_demo DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
USE efcore_demo;

DROP TABLE IF EXISTS orders;
DROP TABLE IF EXISTS users;

CREATE TABLE users (
  id BIGINT PRIMARY KEY AUTO_INCREMENT,
  username VARCHAR(50) NOT NULL,
  email VARCHAR(100) NOT NULL,
  age INT NOT NULL,
  status VARCHAR(20) NOT NULL,
  version INT NOT NULL DEFAULT 0,
  created_at DATETIME(6) NOT NULL,
  updated_at DATETIME(6) NULL,
  UNIQUE KEY uk_users_email (email)
);

CREATE TABLE orders (
  id BIGINT PRIMARY KEY AUTO_INCREMENT,
  user_id BIGINT NOT NULL,
  order_no VARCHAR(50) NOT NULL,
  amount DECIMAL(10, 2) NOT NULL,
  status VARCHAR(20) NOT NULL,
  created_at DATETIME(6) NOT NULL,
  INDEX idx_orders_user_id (user_id),
  CONSTRAINT fk_orders_user FOREIGN KEY (user_id) REFERENCES users (id)
);

INSERT INTO users (username, email, age, status, version, created_at, updated_at) VALUES
('张三', 'zhangsan@example.com', 20, 'ACTIVE', 0, '2026-01-01 10:00:00.000000', NULL),
('李四', 'lisi@example.com', 25, 'ACTIVE', 0, '2026-01-02 10:00:00.000000', NULL),
('王五', 'wangwu@example.com', 17, 'DISABLED', 0, '2026-01-03 10:00:00.000000', NULL);

INSERT INTO orders (user_id, order_no, amount, status, created_at) VALUES
(1, 'A001', 99.00, 'PAID', '2026-02-01 10:00:00.000000'),
(1, 'A002', 260.00, 'PAID', '2026-02-02 10:00:00.000000'),
(2, 'A003', 35.50, 'CANCELLED', '2026-02-03 10:00:00.000000');
