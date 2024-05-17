using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integrir
{
    internal class SQLCODE
    {
        /*
         * 
         * 
         * CREATE TABLE Клиенты (
    client_id SERIAL PRIMARY KEY,
    client_name VARCHAR(50) NOT NULL,
    phone VARCHAR(15) NOT NULL
);

CREATE TABLE Товары (
    id SERIAL PRIMARY KEY,
    name VARCHAR(50) NOT NULL,
    price DECIMAL(10, 2) NOT NULL
);

CREATE TABLE Договор (
    id SERIAL PRIMARY KEY,
    client_id INT NOT NULL,
    signed_date DATE NOT NULL,
    total_sum DECIMAL(10, 2) NOT NULL,
    payment_type VARCHAR(10) CHECK(payment_type IN ('наличка', 'картой')),
    payment_status VARCHAR(20) CHECK(payment_status IN ('оплачен', 'не оплачен', 'внесена предоплата')),
    shipment_status VARCHAR(20) CHECK(shipment_status IN ('отправлен', 'не отправлен', 'готовится к отправке')),
    FOREIGN KEY (client_id) REFERENCES Клиенты(client_id)
);

CREATE TABLE Договор_товары (
    id SERIAL PRIMARY KEY,
    contract_id INT NOT NULL,
    product_id INT NOT NULL,
    quantity INT NOT NULL,
    local_total_sum DECIMAL(10, 2) NOT NULL,
    FOREIGN KEY (contract_id) REFERENCES Договор(id),
    FOREIGN KEY (product_id) REFERENCES Товары(id)
);
         * 
         * 
         * 
         * 
         * CREATE OR REPLACE FUNCTION calculate_local_total_sum()
RETURNS TRIGGER AS $$
BEGIN
    NEW.local_total_sum := (SELECT price * NEW.quantity FROM Товары WHERE id = NEW.product_id);
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER before_insert_local_total_sum
BEFORE INSERT ON Договор_товары
FOR EACH ROW
EXECUTE FUNCTION calculate_local_total_sum();

CREATE OR REPLACE FUNCTION update_total_sum()
RETURNS TRIGGER AS $$
BEGIN
    UPDATE Договор
    SET total_sum = (SELECT SUM(local_total_sum) FROM Договор_товары WHERE contract_id = NEW.contract_id)
    WHERE id = NEW.contract_id;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER after_insert_update_total_sum
AFTER INSERT OR UPDATE ON Договор_товары
FOR EACH ROW
EXECUTE FUNCTION update_total_sum();

CREATE OR REPLACE FUNCTION update_shipment_status()
RETURNS TRIGGER AS $$
BEGIN
    IF NEW.payment_status = 'оплачено' THEN
        NEW.shipment_status := 'отправлено';
    ELSIF NEW.payment_status = 'не оплачен' THEN
        NEW.shipment_status := 'не отправлено';
    ELSE
        NEW.shipment_status := 'готовится к отправке';
    END IF;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER update_shipment_status_trigger
BEFORE INSERT OR UPDATE ON Договор
FOR EACH ROW
EXECUTE FUNCTION update_shipment_status();





        CREATE OR REPLACE FUNCTION update_shipment_status()
RETURNS TRIGGER AS $$
BEGIN
    IF NEW.payment_status = 'оплачен' THEN
        NEW.shipment_status := 'отправлен';
    ELSIF NEW.payment_status = 'не оплачен' THEN
        NEW.shipment_status := 'не отправлен';
    ELSE
        NEW.shipment_status := 'готовится к отправке';
    END IF;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER update_shipment_status_trigger
BEFORE INSERT OR UPDATE ON Договор
FOR EACH ROW
EXECUTE FUNCTION update_shipment_status();

-- Удаление триггера
DROP TRIGGER update_shipment_status_trigger ON Договор;

-- Удаление функции
DROP FUNCTION update_shipment_status();


-- Удаление старых ограничений:
ALTER TABLE Договор DROP CONSTRAINT IF EXISTS Договор_payment_status_check;
ALTER TABLE Договор DROP CONSTRAINT IF EXISTS Договор_shipment_status_check;

-- Добавление новых ограничений:
ALTER TABLE Договор ADD CONSTRAINT payment_status CHECK(payment_status IN ('оплачен', 'не оплачен', 'внесена предоплата'));
ALTER TABLE Договор ADD CONSTRAINT shipment_status CHECK(shipment_status IN ('отправлен', 'не отправлен', 'готовится к отправке'));

Drop Table Договор 

Select * From Договор



         * 
         * 
         * 
         * 
         * 
         * 
         * 
         * 
         * 
         * 
         * 
         * 
         * 
         * 
         * 
         * 
         * 
         * 
         * 
         * 
         */
    }
}
