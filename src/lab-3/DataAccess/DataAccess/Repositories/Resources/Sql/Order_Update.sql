UPDATE orders
SET order_state      = @order_state,
    order_created_at = @order_created_at,
    order_created_by = @order_created_by
WHERE order_id = @order_id;