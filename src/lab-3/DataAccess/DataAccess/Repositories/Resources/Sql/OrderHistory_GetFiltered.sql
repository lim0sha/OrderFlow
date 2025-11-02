SELECT order_history_item_id,
       order_id,
       order_history_item_created_at,
       order_history_item_kind,
       order_history_item_payload
FROM order_history
WHERE order_history_item_id > @cursor
  AND (@order_id IS NULL OR order_id = @order_id)
  AND (@order_history_item_kind IS NULL OR order_history_item_kind = @order_history_item_kind)
ORDER BY order_history_item_id LIMIT @page_size;