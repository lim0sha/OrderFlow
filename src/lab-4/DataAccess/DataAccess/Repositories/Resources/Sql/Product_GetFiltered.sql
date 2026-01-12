SELECT product_id,
       product_name,
       product_price
FROM products
WHERE (product_id > @cursor)
  AND (cardinality(@ids) = 0 OR product_id = ANY (@ids))
  AND (@product_name IS NULL OR product_name LIKE @product_name)
  AND (@min_price IS NULL OR product_price > @min_price)
  AND (@max_price IS NULL OR product_price < @max_price)
ORDER BY product_id LIMIT @page_size;