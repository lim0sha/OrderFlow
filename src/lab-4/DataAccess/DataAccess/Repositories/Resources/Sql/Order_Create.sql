INSERT INTO orders (order_state,
                    order_created_at,
                    order_created_by)
VALUES (@order_state,
        @order_created_at,
        @order_created_by) RETURNING order_id;