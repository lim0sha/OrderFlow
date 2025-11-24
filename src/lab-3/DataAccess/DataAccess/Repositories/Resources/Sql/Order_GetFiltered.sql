SELECT order_id,
       order_state,
       order_created_at,
       order_created_by
FROM orders
WHERE (order_id > @cursor)
  AND (cardinality(@ids) = 0 OR order_id = ANY (@ids))
  AND (@order_state IS NULL OR order_state = @order_state)
  AND (@order_created_by IS NULL OR order_created_by = @order_created_by)
ORDER BY order_id LIMIT @page_size;