SELECT order_item_id,
       order_id,
       product_id,
       order_item_quantity,
       order_item_deleted
FROM order_items
WHERE order_item_id > @cursor
  AND (cardinality(@order_ids) = 0 OR order_id = ANY (@order_ids))
  AND (cardinality(@product_ids) = 0 OR product_id = ANY (@product_ids))
  AND (@order_item_deleted IS NULL OR order_item_deleted = @order_item_deleted)
ORDER BY order_item_id LIMIT @page_size;