SELECT order_item_id,
       order_id,
       product_id,
       order_item_quantity,
       order_item_deleted
FROM order_items
WHERE order_item_id = @id;