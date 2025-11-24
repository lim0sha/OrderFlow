INSERT INTO products (product_name,
                      product_price)
VALUES (@name,
        @price) RETURNING product_id;