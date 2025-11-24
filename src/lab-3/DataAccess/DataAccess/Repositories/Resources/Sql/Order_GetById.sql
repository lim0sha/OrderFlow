SELECT order_id,
       order_state,
       order_created_at,
       order_created_by
FROM orders
WHERE order_id = @id;